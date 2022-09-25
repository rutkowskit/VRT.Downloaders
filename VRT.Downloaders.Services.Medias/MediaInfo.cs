using System;

namespace VRT.Downloaders.Services.Medias;

public sealed class MediaInfo
{
    public string Title { get; set; }
    public Uri Url { get; set; }
    public string Extension { get; set; }
    public string FormatDescription { get; set; }
    public string OutputFileName { get; set; }
}
