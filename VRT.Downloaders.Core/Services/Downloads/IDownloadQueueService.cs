using CSharpFunctionalExtensions;
using DynamicData;
using System.Threading.Tasks;

namespace VRT.Downloaders.Services.Downloads
{
    public interface IDownloadQueueService
    {
        IObservableCache<DownloadTask, string> LiveDownloads { get; }
        Task<Result> AddDownloadTask(DownloadRequest request);
        Task<Result> CancelDownloadTask(DownloadTask task);
    }
}
