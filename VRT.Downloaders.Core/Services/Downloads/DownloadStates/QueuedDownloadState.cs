namespace VRT.Downloaders.Services.Downloads.DownloadStates
{
    public sealed class QueuedDownloadState : BaseRemovableDownloadState
    {
        private readonly IDownloadExecutor _taskExecutor;

        public QueuedDownloadState(IDownloadExecutor taskExecutor)
        {
            _taskExecutor = taskExecutor;
        }
        public override States State => States.DownloadQueued;

        public override Task<Result> Download(IDownloadContext context)
        {
            var newState = new DownloadingDownloadState(_taskExecutor);
            return Transition(context, newState)
                .Bind(() => newState.Download(context));
        }
    }
}
