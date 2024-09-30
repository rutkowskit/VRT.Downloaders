using CommunityToolkit.Mvvm.ComponentModel;
using VRT.Downloaders.Common.Collections;
using VRT.Downloaders.Common.DownloadStates;
using VRT.Downloaders.Common.Models;
using static VRT.Downloaders.Common.DownloadStates.BaseDownloadState;

namespace VRT.Downloaders.Services.DownloadQueue;

public sealed partial class DownloadTask : ObservableObject, IDownloadContext
{
    private readonly SemaphoreSlim _semaphore;
    private BaseDownloadState _currentState;
    public event EventHandler<BaseDownloadState> StateChanged;
    public DownloadTask(DownloadRequest? request)
    {
        _semaphore = new(1, 1);
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
        if (_currentState == state)
        {
            return;
        }
        _semaphore.Wait();
        try
        {
            if (_currentState == state)
            {
                return;
            }
            _currentState = state;
            _currentState.EnterState(this);
            StateChanged(this, state);
        }
        finally
        {
            _semaphore.Release();
        }
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
