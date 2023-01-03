namespace VRT.Downloaders.Common.DownloadStates;

public sealed class ToDownloadDownloadState : BaseRemovableDownloadState
{
    public override States State => States.ToDownload;
}
