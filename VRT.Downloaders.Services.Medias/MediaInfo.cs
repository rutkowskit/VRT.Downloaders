namespace VRT.Downloaders.Services.Medias;

public sealed class MediaInfo
{
    required public string Title { get; init; }
    required public Uri Url { get; init; }
    required public string Extension { get; init; }
    public string? FormatDescription { get; set; }    
    public string? OutputFileName { get; set; }
}
