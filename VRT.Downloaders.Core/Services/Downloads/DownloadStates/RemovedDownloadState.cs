namespace VRT.Downloaders.Services.Downloads.DownloadStates;

public sealed class RemovedDownloadState : BaseDownloadState
{
    public override States State => States.Removed;
}