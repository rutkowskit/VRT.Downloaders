using CSharpFunctionalExtensions;
using DynamicData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using VRT.Downloaders.Properties;
using VRT.Downloaders.Services.Downloads.DownloadStates;

namespace VRT.Downloaders.Services.Downloads
{
    public sealed class DownloadingWorker : IDisposable
    {
        private readonly IDownloadQueueService _downloadService;
        private readonly CompositeDisposable _disposables;
        private readonly ConcurrentQueue<DownloadTask> _downloadTasks;

        public DownloadingWorker(IDownloadQueueService downloadService)
        {
            _disposables = new CompositeDisposable();
            _downloadTasks = new ConcurrentQueue<DownloadTask>();
            _downloadService = downloadService;

            _downloadService.LiveDownloads.Connect()
               .Synchronize()
               .WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
               .Filter(f => f.State == BaseDownloadState.States.ToDownload)
               .Subscribe(changes => EnqueueTasks(changes.Select(change => change.Current)))
               .DisposeWith(_disposables)
               .Discard();

            Observable.StartAsync(StartDownloadingDeamon).Subscribe().DisposeWith(_disposables).Discard();
        }

        private async Task StartDownloadingDeamon()
        {
            while (true)
            {
                if (!_downloadTasks.TryDequeue(out var toDownload))
                {
                    await Task.Delay(1000);
                    continue;
                }

                try
                {
                    await toDownload.Download().Discard();
                }
                catch (Exception ex)
                {
                    var message = Resources.Error_Exception.Format(ex.Message);
                    toDownload.TransitionToState(
                        new FinishedDownloadState(message));
                }
            }
        }

        private void EnqueueTasks(IEnumerable<DownloadTask> tasks)
        {
            var tasksToAdd = tasks?.ToArray() ?? Array.Empty<DownloadTask>();
            foreach (var task in tasksToAdd)
            {
                var executor = new DownloadExecutor()
                    .WithProgressCallback(progress => task.DownloadProgress = progress);

                task.TransitionToState(new QueuedDownloadState(executor));
                _downloadTasks.Enqueue(task);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}