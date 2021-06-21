namespace VRT.Downloaders.Services.Downloads
{
    public sealed class FileByteRange
    {
        public FileByteRange(long from, long to)
        {
            From = from;
            To = to;
        }
        public bool IsDownloaded { get; set; }
        public string LastError { get; set; }
        public long Size => To - From;
        public long From { get; }
        public long To { get; }
    }
}
