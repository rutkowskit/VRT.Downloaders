using VRT.Downloaders.Common.Abstractions;

namespace VRT.Downloaders.Common.DownloadStates;

public sealed class DownloadingDownloadState : BaseDownloadState
{
    private readonly IDownloadExecutorService _taskExecutor;

    public DownloadingDownloadState(IDownloadExecutorService taskExecutor)
    {
        _taskExecutor = taskExecutor;
    }
    public override States State => States.Downloading;
    public override bool CanCancel => true;
    public override Result Cancel(IDownloadContext context)
    {
        return _taskExecutor.Cancel()
            .Bind(() => Transition(context, new CancelingDownloadState("Canceled by user")));
    }
    public override Task<Result> Download(IDownloadContext context)
    {
        return Result.Success()
            .BindTry(() => _taskExecutor.Download(context))
            .Bind(() => TransitionToFinishedState(context))
            .TapError(error => TransitionToErrorState(context, error));
    }
    private Result TransitionToFinishedState(IDownloadContext context)
    {
        return Transition(context, new FinishedDownloadState(Resources.Msg_Success));
    }
    private Result TransitionToErrorState(IDownloadContext context, string error)
    {
        return Transition(context, new ErrorDownloadState(error));
    }
}