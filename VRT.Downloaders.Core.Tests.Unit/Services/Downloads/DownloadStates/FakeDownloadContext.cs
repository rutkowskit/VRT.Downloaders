using CSharpFunctionalExtensions;

namespace VRT.Downloaders.Services.Downloads.DownloadStates
{
    internal sealed class FakeDownloadContext : IDownloadContext
    {
        public BaseDownloadState CurrentState { get; private set; }
        public DownloadRequest Request { get; }
        public BaseDownloadState.States State { get; set; }
        public string LastErrorMessage { get; set; }
        public bool CanCancel { get; set; }
        public bool CanRemove { get; set; }
        public FilePartitions Partitions { get; set; }

        public void TransitionToState(BaseDownloadState state)
        {
            CurrentState = state;
            CurrentState.EnterState(this)
                .OnFailure(f => LastErrorMessage = f);
        }
    }
}
