using VRT.Downloaders.Common.DownloadStates;

namespace VRT.Downloaders.Common.Abstractions;

public interface IDownloadExecutorService
{
    Result Cancel();
    Task<Result> Download(IDownloadContext task);
    IDownloadExecutorService WithProgressCallback(Action<int> progressCallback);
}