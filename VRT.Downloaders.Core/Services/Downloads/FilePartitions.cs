namespace VRT.Downloaders.Services.Downloads
{
    public sealed class FilePartitions
    {
        public FilePartitions(Uri resourcePath, long fileSize, FileByteRange[] partitions)
        {
            ResourcePath = resourcePath;
            FileSize = fileSize;
            Partitions = partitions;
        }
        public long DownloadedSize
        {
            get
            {
                if (FileSize <= 0 || Partitions == null || Partitions.Length == 0)
                {
                    return 0;
                }
                return Partitions
                    .Where(p => p.IsDownloaded)
                    .Sum(p => p.Size);
            }
        }
        public int DownloadedPercent
        {
            get => FileSize <= 0
                ? 0
                : ((int)(((double)DownloadedSize / FileSize) * 100)) % 101;
        }
        public Uri ResourcePath { get; }
        public long FileSize { get; }
        public FileByteRange[] Partitions { get; }
    }
}
