using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using VRT.Assets.Application.Common.Abstractions;
using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Common.Collections;
using VRT.Downloaders.Common.DownloadStates;
using VRT.Downloaders.Common.Options;
using VRT.Downloaders.Services.DownloadQueue;

namespace VRT.Downloaders.Workers;

public sealed class DownloadingWorker : IDisposable
{
    private readonly IDownloadQueueService _downloadQueueService;
    private readonly CompositeDisposable _disposables;
    private readonly ConcurrentQueue<DownloadTask> _downloadTasksQueue;
    private readonly IAbstractFactory<IDownloadExecutorService> _downloadExecutorFactory;
    private volatile bool _disposed;
    private readonly DownloadingWorkerOptions _workerOptions;
    public DownloadingWorker(
        IDownloadQueueService downloadQueueService,
        IAbstractFactory<IDownloadExecutorService> downloadExecutorFactory,
        IOptions<DownloadingWorkerOptions> workerOptions)
    {
        _downloadQueueService = Guard.AgainstNull(downloadQueueService);
        _downloadExecutorFactory = Guard.AgainstNull(downloadExecutorFactory);
        _disposables = new CompositeDisposable();
        _downloadTasksQueue = new();
        _workerOptions = workerOptions.Value;

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

        Observable.StartAsync(StartDownloadingDeamon)
            .Subscribe()
            .DisposeWith(_disposables);

    }
    public bool IsDisposed => _disposed;
    public int DownloadsCount => _downloadTasksQueue.Count;

    private async Task StartDownloadingDeamon(CancellationToken cancellationToken)
    {
        var downloadingTasks = new BlockingList<Task<Result<string>>>();
        Task<Result<IReadOnlyCollection<string>>>? downloadTask = null;
        
        while (cancellationToken.IsCancellationRequested is false)
        {
            var isMaxTaskCountOutOfRange = downloadingTasks.Count >= _workerOptions.MaxConcurrentDownloads;
            if (isMaxTaskCountOutOfRange || _downloadTasksQueue.TryDequeue(out var toDownload) == false)
            {
                await DelayIdleTime(cancellationToken);                                
                continue;
            }
            var downloadingTask = toDownload.Download()
                .Finally(r => HandleDownloadResult(toDownload, r));

            downloadingTasks.Add(downloadingTask);

            if(downloadTask is null || downloadTask.IsCompleted)
            {
                downloadTask = downloadingTasks
                    .DoParallel(continueOnFailure: true, cancellationToken: cancellationToken);
            }            
        }
    }
    private async Task DelayIdleTime(CancellationToken cancellationToken)
    {
        await Task
            .Delay(_workerOptions.IdleDelayTimeMilliseconds, cancellationToken)
            .ContinueWith(_ => { }, cancellationToken);
    }
    private static Result<string> HandleDownloadResult(DownloadTask finishedTask, Result taskResult)
    {
        if(taskResult.IsFailure)
        {
            var message = Resources.Error_Exception.Format(taskResult.Error);
            finishedTask.TransitionToState(new ErrorDownloadState(message));
        }
        return taskResult.IsSuccess
            ? Result.Success("OK")
            : Result.Failure<string>(taskResult.Error);
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
        var executor = _downloadExecutorFactory.Create()
            .WithProgressCallback(progress => task.DownloadProgress = progress);

        task.TransitionToState(new QueuedDownloadState(executor));
        _downloadTasksQueue.Enqueue(task);
    }

    public void Dispose()
    {       
        _disposables.Dispose();
        _disposed = true;
    }
}