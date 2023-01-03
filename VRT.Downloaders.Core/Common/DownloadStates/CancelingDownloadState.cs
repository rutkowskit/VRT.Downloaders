namespace VRT.Downloaders.Common.DownloadStates;

public sealed class CancelingDownloadState : BaseDownloadState
{
    public CancelingDownloadState(string reason)
    {
        Reason = reason;
    }
    public string Reason { get; }
    public override States State => States.Canceling;

    public override Result EnterState(IDownloadContext context)
    {
        return base.EnterState(context)
            .Tap(() => context.LastErrorMessage = Reason);
    }
}
