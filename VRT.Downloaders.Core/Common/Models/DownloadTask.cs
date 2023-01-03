using CommunityToolkit.Mvvm.ComponentModel;
using VRT.Downloaders.Common.Collections;
using VRT.Downloaders.Common.DownloadStates;
using VRT.Downloaders.Common.Models;
using static VRT.Downloaders.Common.DownloadStates.BaseDownloadState;

namespace VRT.Downloaders.Services.DownloadQueue;

public sealed partial class DownloadTask : ObservableObject, IDownloadContext
{
    private BaseDownloadState _currentState;
    public event EventHandler<BaseDownloadState> StateChanged;
    public DownloadTask(DownloadRequest? request)
    {
        StateChanged = delegate { };
        Request = request ?? throw new ArgumentNullException(nameof(request));
        _currentState = null!;
        var initialState = new ToDownloadDownloadState();
        TransitionToState(initialState);
    }
    public DownloadRequest Request { get; }
    public FilePartitions? Partitions { get; set; }

    [ObservableProperty] private States _state;
    [ObservableProperty] private int _downloadProgress;
    [ObservableProperty] private string? _lastErrorMessage;
    [ObservableProperty] private bool _canCancel;
    [ObservableProperty] private bool _canRemove;

    public void TransitionToState(BaseDownloadState state)
    {
        //TODO: add locking to avoid race condition problem
        _currentState = state;
        _currentState.EnterState(this);
        StateChanged(this,state);
    }
    public Task<Result> Download()
    {
        return _currentState.Download(this);
    }

    public Task<Result> Cancel()
    {
        return Task.FromResult(_currentState.Cancel(this));
    }

    public Task<Result> Remove()
    {
        return Task.FromResult(_currentState.Remove(this));
    }
}
