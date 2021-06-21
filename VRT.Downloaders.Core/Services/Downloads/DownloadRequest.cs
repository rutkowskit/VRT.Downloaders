using System;

namespace VRT.Downloaders.Services.Downloads
{
    public sealed class DownloadRequest
    {
        public DownloadRequest(string name, Uri uri, string outputFileName)
        {
            Name = name;
            Uri = uri;
            OutputFileName = outputFileName;
        }

        public string Name { get; }
        public Uri Uri { get; }
        public string OutputFileName { get; }
    }
}
