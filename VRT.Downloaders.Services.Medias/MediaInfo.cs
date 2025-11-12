namespace VRT.Downloaders.Services.Medias;

public sealed class MediaInfo
{
    public static MediaInfo Empty { get; } = new MediaInfo
    {
        Title = string.Empty,
        Url = new Uri("http://localhost/"),
        Extension = string.Empty
    };
    required public string Title { get; init; }
    required public Uri Url { get; init; }
    required public string Extension { get; init; }
    public string? FormatDescription { get; set; }    
    public string? OutputFileName { get; set; }
}
