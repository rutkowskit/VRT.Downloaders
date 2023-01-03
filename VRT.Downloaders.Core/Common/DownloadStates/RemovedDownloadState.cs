namespace VRT.Downloaders.Common.DownloadStates;

public sealed class RemovedDownloadState : BaseDownloadState
{
    public override States State => States.Removed;
}