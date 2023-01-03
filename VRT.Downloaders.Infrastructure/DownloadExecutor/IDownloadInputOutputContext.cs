namespace VRT.Downloaders.Infrastructure.DownloadExecutor;

public interface IDownloadInputOutputContext : IDisposable
{
    public SemaphoreSlim ReadSemaphore { get; }
    public SemaphoreSlim WriteSemaphore { get; }
    public FileStream OutputStream { get; }
    public CancellationToken CancellationToken { get; set; }
}
