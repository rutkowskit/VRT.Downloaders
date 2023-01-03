using VRT.Downloaders.Common.Abstractions;

namespace VRT.Downloaders.Common.DownloadStates;

public sealed class QueuedDownloadState : BaseRemovableDownloadState
{
    private readonly IDownloadExecutorService _taskExecutor;

    public QueuedDownloadState(IDownloadExecutorService taskExecutor)
    {
        _taskExecutor = taskExecutor;
    }
    public override States State => States.DownloadQueued;

    public override Task<Result> Download(IDownloadContext context)
    {
        var newState = new DownloadingDownloadState(_taskExecutor);
        return Transition(context, newState)
            .Bind(() => newState.Download(context));
    }
}
