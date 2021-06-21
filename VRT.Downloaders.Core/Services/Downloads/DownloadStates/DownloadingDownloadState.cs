using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace VRT.Downloaders.Services.Downloads.DownloadStates
{
    public sealed class DownloadingDownloadState : BaseDownloadState
    {
        private readonly DownloadExecutor _taskExecutor;

        public DownloadingDownloadState(DownloadExecutor taskExecutor)
        {
            _taskExecutor = taskExecutor;
        }
        public override States State => States.Downloading;
        public override bool CanCancel => true;
        public override Result Cancel(IDownloadContext context)
        {
            return _taskExecutor.Cancel()
                .Bind(() => Transition(context, new CancelingDownloadState("Canceled by user")));
        }
        public override Task<Result> Download(IDownloadContext context)
        {            
            return _taskExecutor.Download(context)
                .Finally(result => TransitionToFinishedState(context, result));
        }
        private Result TransitionToFinishedState(IDownloadContext context, Result result)
        {
            return Transition(context, new FinishedDownloadState(result.IsSuccess ? "Success" : result.Error))
                .Bind(() => result);            
        }
    }
}