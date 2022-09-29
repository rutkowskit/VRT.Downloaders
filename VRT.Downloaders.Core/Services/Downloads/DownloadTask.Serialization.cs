using System.Runtime.Serialization;

namespace VRT.Downloaders.Services.Downloads
{
    [Serializable]
    partial class DownloadTask : ISerializable
    {
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Request), Request);
            info.AddValue(nameof(Partitions), Partitions);
            info.AddValue(nameof(State), State);
            info.AddValue(nameof(LastErrorMessage), LastErrorMessage);
        }

        private DownloadTask(SerializationInfo info, StreamingContext context)
            : this((DownloadRequest)info.GetValue(nameof(Request), typeof(DownloadRequest)))
        {
            var state = (BaseDownloadState.States)info.GetValue(nameof(State), typeof(BaseDownloadState.States));
            TransitionToState(CreateState(state));
            Partitions = (FilePartitions)info.GetValue(nameof(Partitions), typeof(FilePartitions));
            LastErrorMessage = (string)info.GetValue(nameof(LastErrorMessage), typeof(string));
            DownloadProgress = Partitions?.DownloadedPercent ?? 0;
        }

        private static BaseDownloadState CreateState(BaseDownloadState.States state)
        {
            switch (state)
            {
                case BaseDownloadState.States.Canceling: return new FinishedDownloadState("Canceled");
                case BaseDownloadState.States.Finished: return new FinishedDownloadState("");
                case BaseDownloadState.States.Removed: return new RemovedDownloadState();
                default: return new ToDownloadDownloadState();
            }
        }
    }
}
