namespace VRT.Downloaders.Services.Downloads.DownloadStates
{
    public sealed class ErrorDownloadState : BaseRemovableDownloadState
    {
        public ErrorDownloadState(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
        public override States State => States.Error;

        public string ErrorMessage { get; }
        public override Result EnterState(IDownloadContext context)
        {
            return base.EnterState(context)
                .Tap(() => context.LastErrorMessage = ErrorMessage);
        }
        public override Task<Result> Download(IDownloadContext context)
        {
            var newState = Transition(context, new ToDownloadDownloadState());
            return Task.FromResult(newState);
        }
        public override Result Remove(IDownloadContext context)
        {
            return Transition(context, new RemovedDownloadState());
        }
    }
}