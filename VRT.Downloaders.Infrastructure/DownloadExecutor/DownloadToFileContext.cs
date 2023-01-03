
namespace VRT.Downloaders.Infrastructure.DownloadExecutor;
internal sealed class DownloadToFileInputOutputContext : IDownloadInputOutputContext
{
    public SemaphoreSlim ReadSemaphore { get; }
    public SemaphoreSlim WriteSemaphore { get; }
    public FileStream OutputStream { get; }
    public CancellationToken CancellationToken { get; set; }

    public DownloadToFileInputOutputContext(string outputFileName, int maxReads = 4, int maxWrites = 1, 
        CancellationToken cancellationToken = default)
    {
        maxReads = Math.Max(1, maxReads);
        maxWrites = Math.Max(1, maxWrites);

        ReadSemaphore = new SemaphoreSlim(maxReads, maxReads); // multiple reads
        WriteSemaphore = new SemaphoreSlim(maxWrites, maxWrites); // single write
        OutputStream = new FileStream(outputFileName, FileMode.OpenOrCreate);
        CancellationToken = cancellationToken;
    }
    public void Dispose()
    {
        OutputStream.Flush(true);
        OutputStream.Dispose();
        ReadSemaphore.Dispose();
        WriteSemaphore.Dispose();
    }
}