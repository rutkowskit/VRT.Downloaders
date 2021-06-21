using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace VRT.Downloaders.Services.Downloads.DownloadStates
{
    public sealed class QueuedDownloadState : BaseRemovableDownloadState
    {
        private readonly DownloadExecutor _taskExecutor;

        public QueuedDownloadState(DownloadExecutor taskExecutor)
        {
            _taskExecutor = taskExecutor;
        }
        public override States State => States.DownloadQueued;

        public override Result Cancel(IDownloadContext context)
        {
            return Transition(context, new FinishedDownloadState("Canceled before start"));
        }
        public override Task<Result> Download(IDownloadContext context)
        {
            var newState = new DownloadingDownloadState(_taskExecutor);
            return Transition(context, newState)
                .Bind(() => newState.Download(context));
        }
    }
}
