namespace VRT.Downloaders.Common.Options;
public sealed class DownloadingWorkerOptions
{
    public int IdleDelayTimeMilliseconds { get; set; } = 1000;
    public int MaxConcurrentDownloads { get; set; } = 4;
}
