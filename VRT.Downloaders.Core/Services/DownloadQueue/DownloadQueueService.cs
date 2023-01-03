using ReactiveUI;
using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Common.DownloadStates;
using VRT.Downloaders.Common.Models;

namespace VRT.Downloaders.Services.DownloadQueue;

public sealed class DownloadQueueService : IDownloadQueueService, IDisposable
{
    public static readonly string DownloadQueueTasksStateKey = "State_DownloadQueueTasks";
    private readonly SourceCache<DownloadTask, string> _downloads;
    private readonly CompositeDisposable _disposables;
    private readonly IAppStateService _appStateService;
    private readonly SemaphoreSlim _addTaskSemaphore;
    public DownloadQueueService(IAppStateService appStateService)
    {
        _addTaskSemaphore = new SemaphoreSlim(1, 1);
        _disposables = new CompositeDisposable();
        _downloads = new SourceCache<DownloadTask, string>(d => d.Request.Uri.AbsoluteUri);
        LiveDownloads = _downloads.AsObservableCache();
        _disposables.Add(LiveDownloads);
        _appStateService = appStateService;
        RestoreDownloadQueueState();
    }

    public IObservableCache<DownloadTask, string> LiveDownloads { get; }

    public async Task<Result> AddDownloadTask(DownloadRequest request)
    {
        var result = Result.SuccessIf(request is not null, Resources.Error_TaskCannotBeNull);
        try
        {
            await _addTaskSemaphore.WaitAsync();
            return result
                .Bind(() => EnsureNotInCache(_downloads, request!))
                .Map(req => new DownloadTask(req))
                .Tap(task => RemoveFromCacheWhenStateChangedToRemoved(_downloads, task))
                .Tap(task => _downloads.AddOrUpdate(task));
        }
        finally
        {
            _addTaskSemaphore.Release();
        };
    }

    public void Dispose()
    {
        StoreDownloadQueueState();
        _disposables.Dispose();
    }

    public Task StoreDownloadQueueState()
    {
        var toStore = _downloads.Items
            .Where(t => t.State != BaseDownloadState.States.Removed)
            .ToArray();

        _appStateService?.Store(DownloadQueueTasksStateKey, toStore);

        return Task.CompletedTask;
    }
    private void RestoreDownloadQueueState()
    {
        if (_appStateService == null || _downloads == null)
        {
            return;
        }
        var tasks = _appStateService.Restore<DownloadTask[]>(DownloadQueueTasksStateKey)
            ?? Array.Empty<DownloadTask>();

        foreach (var task in tasks)
        {
            RemoveFromCacheWhenStateChangedToRemoved(_downloads, task);
            _downloads.AddOrUpdate(task);
        }
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
