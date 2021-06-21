using System;
using System.IO;
using System.Threading;

namespace VRT.Downloaders.Services.Downloads
{
    public sealed class DownloadToFileContext : IDisposable
    {
        public SemaphoreSlim ReadSemaphore { get; }
        public SemaphoreSlim WriteSemaphore { get; }
        public FileStream OutputStream { get; }
        public CancellationToken CancellationToken { get; set; }

        public DownloadToFileContext(string outputFileName, CancellationToken cancellationToken = default,
            int maxReads = 4, int maxWrites = 1)
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
}