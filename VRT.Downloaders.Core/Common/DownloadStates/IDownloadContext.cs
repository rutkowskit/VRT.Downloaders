using VRT.Downloaders.Common.Collections;
using VRT.Downloaders.Common.Models;

namespace VRT.Downloaders.Common.DownloadStates;

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
