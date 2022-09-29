using System.Runtime.CompilerServices;

namespace VRT.Downloaders.Services.Downloads.DownloadStates
{
    public abstract class BaseDownloadState
    {
        public enum States
        {
            Unspecified = 0,
            ToDownload = 1,
            DownloadQueued = 2,
            Downloading = 3,
            Canceling = 4,
            Removed = 10,
            Finished = 100,
            Error = 1000
        }
        public abstract States State { get; }
        public virtual bool CanCancel => false;
        public virtual bool CanRemove => false;
        public virtual Result EnterState(IDownloadContext context)
        {
            return EnsureDifferentState(context, State)
                .Tap(ctx =>
                {
                    ctx.State = State;
                    ctx.LastErrorMessage = "";
                    ctx.CanCancel = CanCancel;
                    ctx.CanRemove = CanRemove;
                });
        }

        public virtual Task<Result> Download(IDownloadContext context)
        {
            return Task.FromResult(CreateNotSupportedFailure());
        }
        public virtual Result Cancel(IDownloadContext context)
        {
            return CreateNotSupportedFailure();
        }
        public virtual Result Remove(IDownloadContext context)
        {
            return CreateNotSupportedFailure();
        }

        protected static Result CreateNotSupportedFailure(string reason = null,
            string methodName = null, [CallerMemberName] string caller = "")
        {
            var message = Resources.Error_MethodNotSupported_Name_Reason.Format(methodName ?? caller, reason);
            return Result.Failure(message);
        }

        protected Result<IDownloadContext> EnsureDifferentState(IDownloadContext context, States newState)
        {
            return context.State != newState
                ? Result.Success(context)
                : Result.Failure<IDownloadContext>(Resources.Error_StateAlreadySet_StateName.Format(context.State));
        }

        protected Result Transition(IDownloadContext context, BaseDownloadState newState)
        {
            return EnsureDifferentState(context, newState.State)
                .Tap(ctx => ctx.TransitionToState(newState));
        }
    }
}
