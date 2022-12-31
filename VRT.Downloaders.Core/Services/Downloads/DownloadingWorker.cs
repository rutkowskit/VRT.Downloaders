using System.Collections.Concurrent;

namespace VRT.Downloaders.Services.Downloads;

public sealed class DownloadingWorker : IDisposable
{
    private readonly IDownloadQueueService _downloadQueueService;
    private readonly CompositeDisposable _disposables;
    private readonly ConcurrentQueue<DownloadTask> _downloadTasks;

    public DownloadingWorker(IDownloadQueueService downloadQueueService)
    {
        _disposables = new CompositeDisposable();
        _downloadTasks = new();
        _downloadQueueService = downloadQueueService;

        var shared = _downloadQueueService.LiveDownloads.Connect().Publish();
        shared
            .Synchronize()
            .WhenPropertyChanged(p => p.State)
            .Where(p => p.Value == BaseDownloadState.States.ToDownload)
            .Subscribe(t => EnqueueTask(t.Sender));

        shared
           .Synchronize()
           .WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
           .Filter(f => f.State == BaseDownloadState.States.ToDownload)
           .Subscribe(changes => EnqueueTasks(changes.Select(change => change.Current)));

        shared.Connect().DisposeWith(_disposables);

        Observable.StartAsync(StartDownloadingDeamon).Subscribe().DisposeWith(_disposables);
    }

    private async Task StartDownloadingDeamon()
    {
        var downloadTasks = new BlockingList<Task<Result<string>>>();
        Task<Result<IReadOnlyCollection<string>>>? downloadTask = null;
        while (true)
        {
            //TODO: Add maksimum concurrent downloads download task into worker settings
            if (downloadTasks.Count >= 4 || _downloadTasks.TryDequeue(out var toDownload) == false)
            {
                await Task.Delay(1000);
                continue;
            }
            downloadTasks.Add(toDownload.Download().Finally(r => r.IsSuccess ? Result.Success("OK") : Result.Failure<string>(r.Error)));

            if (downloadTask != null)
            {
                if (downloadTask.IsCompleted == false)
                {
                    continue;
                }
                downloadTask = null;
            }
            downloadTask = downloadTasks.DoParallel(onFailure: error =>
            {
                var message = Resources.Error_Exception.Format(error);
                toDownload.TransitionToState(new ErrorDownloadState(message));
            }, continueOnFailure: true);
        }
    }

    private void EnqueueTasks(IEnumerable<DownloadTask> tasks)
    {
        var tasksToAdd = tasks?.ToArray() ?? Array.Empty<DownloadTask>();
        foreach (var task in tasksToAdd)
        {
            EnqueueTask(task);
        }
    }
    private void EnqueueTask(DownloadTask task)
    {
        var executor = new DownloadExecutor()
            .WithProgressCallback(progress => task.DownloadProgress = progress);

        task.TransitionToState(new QueuedDownloadState(executor));
        _downloadTasks.Enqueue(task);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}