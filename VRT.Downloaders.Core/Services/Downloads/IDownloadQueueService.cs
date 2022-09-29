namespace VRT.Downloaders.Services.Downloads;

public interface IDownloadQueueService
{
    IObservableCache<DownloadTask, string> LiveDownloads { get; }
    Task<Result> AddDownloadTask(DownloadRequest request);    
    Task StoreDownloadQueueState();

}
