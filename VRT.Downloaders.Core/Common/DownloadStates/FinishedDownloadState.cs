namespace VRT.Downloaders.Common.DownloadStates;

public sealed class FinishedDownloadState : BaseDownloadState
{
    public FinishedDownloadState(string reason)
    {
        Reason = reason;
    }
    public string Reason { get; }
    public override bool CanRemove => true;
    public override States State => States.Finished;
    public override Result EnterState(IDownloadContext context)
    {
        return base.EnterState(context)
            .Tap(() => context.LastErrorMessage = Reason);
    }
    public override Result Remove(IDownloadContext context)
    {
        return Transition(context, new RemovedDownloadState());
    }
}
