using VRT.Downloaders.Common.Collections;
using VRT.Downloaders.Common.DownloadStates;
using VRT.Downloaders.Common.Models;

namespace VRT.Downloaders.Fakes;

internal sealed class FakeDownloadContext : IDownloadContext
{
    public BaseDownloadState CurrentState { get; private set; } = null!;
    public DownloadRequest Request { get; } = null!;
    public BaseDownloadState.States State { get; set; }
    public string? LastErrorMessage { get; set; }
    public bool CanCancel { get; set; }
    public bool CanRemove { get; set; }
    public FilePartitions? Partitions { get; set; }

    public void TransitionToState(BaseDownloadState state)
    {
        CurrentState = state;
        CurrentState.EnterState(this).TapError(f => LastErrorMessage = f);
    }
}
