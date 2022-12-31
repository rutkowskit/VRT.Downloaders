namespace VRT.Downloaders.Services.Downloads.DownloadStates;

public sealed class ToDownloadDownloadState : BaseRemovableDownloadState
{
    public override States State => States.ToDownload;
}
