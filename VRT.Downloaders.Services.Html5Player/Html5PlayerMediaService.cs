using CSharpFunctionalExtensions;
using System.Net;
using System.Text.RegularExpressions;
using VRT.Downloaders.Services.Medias.Properties;

namespace VRT.Downloaders.Services.Medias.TvpVod;

public sealed partial class Html5PlayerMediaService : IMediaService
{
    public async Task<Result<MediaInfo[]>> GetAvailableMedias(string resourceUrl)
    {
        var result = await CreateUri(resourceUrl)
            .MapTry(url => url.GetWebFile(userAgent: null))
            .Map(ParseHtmp5PlayerLinks);
        return result;
    }
    public async Task<Result> CanGetMedia(string resourceUrl)
    {
        var result = await CreateUri(resourceUrl)
            .MapTry(url => url.GetWebFile(userAgent:null))
            .Ensure(ContainsHtml5PlayerLinks, Resources.Error_MediaNotSupported);        
        return result;
    }
    public int GetServicePriority() => 999;

    private static Result<Uri> CreateUri(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            ? uri
            : Result.Failure<Uri>(Resources.Error_MediaNotSupported);
    }
    private static bool ContainsHtml5PlayerLinks(string webContent)
    {
        return Regex.IsMatch(webContent ?? "", @"\.setVideoUrl", RegexOptions.IgnoreCase | RegexOptions.NonBacktracking);
    }

    private MediaInfo[] ParseHtmp5PlayerLinks(string webContent)
    {
        var title = GetVideoTitle(webContent);
        var result = Regex.Matches(webContent,
            @"(?:\.setVideoUrl(?:low|high)\s*\('(?<url>.*?)\s*'\)\s*;)|(?:\""contentUrl\"".*?\""\s*(?<url>.*?)\s*\""\s*)", 
            RegexOptions.IgnoreCase | RegexOptions.NonBacktracking)
            .Where(m => m.Success)
            .Select(m => m.Groups["url"].Value)
            .Where(url => string.IsNullOrWhiteSpace(url) is false)            
            .Where(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .Distinct()
            .Select(url => ToMediaInfo(url, title))
            .ToArray();
        return result;
    }
    private static MediaInfo ToMediaInfo(string url, string title)
    {
        var fullExt = Path.GetExtension(url)?.TrimStart('.') ?? "mp4";
        var match = Regex.Match(fullExt, @"\.?(?<ext>[^\.\?\r\n]+).*?$", RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);
        var extension = match.Success
            ? match.Groups["ext"].Value
            : fullExt;
        var result = new MediaInfo()
        {
            Extension = extension,
            Title = title,
            Url = new Uri(url),
            OutputFileName = title.SanitizeAsFileName(),
            FormatDescription = extension
        };
        return result;
    }
    private static string GetVideoTitle(string webContent)
    {
        var match = Regex.Match(webContent, @"\.setVideoTitle.*?\('(?<title>.{1,})\s*'\)\s*;", RegexOptions.IgnoreCase | RegexOptions.NonBacktracking);
        return match.Success
            ? match.Groups["title"].Value
            : "Unknown title";
    }
}
