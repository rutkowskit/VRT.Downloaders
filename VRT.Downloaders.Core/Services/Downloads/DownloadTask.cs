using VRT.Downloaders.ViewModels;
using static VRT.Downloaders.Services.Downloads.DownloadStates.BaseDownloadState;

namespace VRT.Downloaders.Services.Downloads
{
    public sealed partial class DownloadTask : BaseViewModel, IDownloadContext
    {
        private BaseDownloadState _currentState;
        public DownloadTask(DownloadRequest request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            TransitionToState(new ToDownloadDownloadState());
        }
        public DownloadRequest Request { get; }
        public FilePartitions Partitions { get; set; }

        [Reactive] public States State { get; set; }
        [Reactive] public int DownloadProgress { get; set; }
        [Reactive] public string LastErrorMessage { get; set; }
        [Reactive] public bool CanCancel { get; set; }
        [Reactive] public bool CanRemove { get; set; }

        public void TransitionToState(BaseDownloadState state)
        {
            //TODO: add locking to avoid race condition problem
            _currentState = state;
            _currentState.EnterState(this).Discard();
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
}
