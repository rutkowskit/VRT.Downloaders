namespace VRT.Downloaders.Services.Downloads.DownloadStates
{
    public abstract class BaseRemovableDownloadState : BaseDownloadState
    {
        public override bool CanRemove => true;
        public override Result Remove(IDownloadContext context)
        {
            return Transition(context, new RemovedDownloadState());
        }
    }
}
