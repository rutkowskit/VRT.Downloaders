using VRT.Downloaders.Common.Models;
using VRT.Downloaders.Services.DownloadQueue;

namespace VRT.Downloaders.Common.Abstractions;

public interface IDownloadQueueService
{
    IObservableCache<DownloadTask, string> LiveDownloads { get; }
    Task<Result> AddDownloadTask(DownloadRequest request);
    Task StoreDownloadQueueState();

}
