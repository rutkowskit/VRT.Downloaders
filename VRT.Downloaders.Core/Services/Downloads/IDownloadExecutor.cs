namespace VRT.Downloaders.Services.Downloads;

public interface IDownloadExecutor
{
    Result Cancel();
    Task<Result> Download(IDownloadContext task);
}