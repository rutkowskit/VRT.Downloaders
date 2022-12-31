namespace VRT.Downloaders.Services.Downloads.DownloadStates;

public interface IDownloadContext
{
    DownloadRequest Request { get; }
    BaseDownloadState.States State { get; set; }
    string? LastErrorMessage { get; set; }
    bool CanCancel { get; set; }
    bool CanRemove { get; set; }
    void TransitionToState(BaseDownloadState state);
    FilePartitions? Partitions { get; set; }
}
