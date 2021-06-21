using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using VRT.Downloaders.Properties;
using VRT.Downloaders.Services.AppStates;
using VRT.Downloaders.Services.Downloads.DownloadStates;

namespace VRT.Downloaders.Services.Downloads
{
    public sealed class DownloadQueueService : IDownloadQueueService, IDisposable
    {
        private readonly SourceCache<DownloadTask, string> _downloads;
        private readonly CompositeDisposable _disposables;
        private readonly IAppStateService _appStateService;
        private const string _downloadQueueTasksStateKey = "State_DownloadQueueTasks";
        public DownloadQueueService(IAppStateService appStateService)
        {
            _disposables = new CompositeDisposable();
            _downloads = new SourceCache<DownloadTask, string>(d => d.Request.Uri.AbsoluteUri);
            LiveDownloads = _downloads.AsObservableCache();
            _disposables.Add(LiveDownloads);
            _appStateService = appStateService;

            RestoreDownloadQueueState();
        }

        private void RestoreDownloadQueueState()
        {
            if (_appStateService == null || _downloads == null)
            {
                return;
            }
            var tasks = _appStateService.Restore<DownloadTask[]>(_downloadQueueTasksStateKey)
                ?? Array.Empty<DownloadTask>();

            foreach (var task in tasks)
            {
                RemoveFromCacheWhenStateChangedToRemoved(_downloads, task);
                _downloads.AddOrUpdate(task);
            }
        }

        public IObservableCache<DownloadTask, string> LiveDownloads { get; }

        public async Task<Result> AddDownloadTask(DownloadRequest request)
        {
            await Task.Yield();
            var result = request == null
                ? Result.Failure(Resources.Error_TaskCannotBeNull)
                : Result.Success();

            return result
                .Bind(() => EnsureNotInCache(_downloads, request))
                .Map(req => new DownloadTask(req))
                .Tap(task => RemoveFromCacheWhenStateChangedToRemoved(_downloads, task))
                .Tap(task => _downloads.AddOrUpdate(task));
        }

        public Task<Result> CancelDownloadTask(DownloadTask task)
        {
            if (!task.CanCancel)
            {
                return Task.FromResult(Result.Failure(Resources.Error_CannotCancelTask));
            }
            return task.Cancel().Tap(() => _downloads.Refresh(task));
        }

        public void Dispose()
        {
            var toStore = _downloads.Items
                .Where(t => t.State != BaseDownloadState.States.Removed)
                .ToArray();

            _appStateService?.Store(_downloadQueueTasksStateKey, toStore);
            _disposables.Dispose();
        }

        private static Result<DownloadRequest> EnsureNotInCache(ISourceCache<DownloadTask, string> cache, DownloadRequest request)
        {
            var existing = cache.Lookup(request.Uri.AbsoluteUri);
            return existing.HasValue && existing.Value.State != BaseDownloadState.States.Removed
                ? Result.Failure<DownloadRequest>(Resources.Error_TaskAlreadyOnDownloadList)
                : Result.Success(request);
        }

        private static void RemoveFromCacheWhenStateChangedToRemoved(ISourceCache<DownloadTask, string> cache, DownloadTask task)
        {
            task.WhenAnyValue(v => v.State)
                .Where(v => v is BaseDownloadState.States.Removed)
                .Subscribe(v => cache.Remove(task))
                .Discard();
        }
    }
}
