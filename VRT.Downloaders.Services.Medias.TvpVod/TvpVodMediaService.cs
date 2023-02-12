using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;
using VRT.Downloaders.Services.Medias.Properties;

namespace VRT.Downloaders.Services.Medias.TvpVod;

public sealed class TvpVodMediaService : IMediaService
{
    private static Regex UrlMatchingRegex = new Regex(
        @"http.*?vod\.tvp\.pl.*?video.*,*?,(?<id>\d+)",
        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

    public async Task<Result<MediaInfo[]>> GetAvailableMedias(string resourceUrl)
    {
        var match = UrlMatchingRegex.Match(resourceUrl ?? "");
        if (!match.Success)
            return Result.Failure<MediaInfo[]>(Resources.Error_MediaNotSupported);

        var id = match.Groups["id"].Value;

        var tokenUrl = $"https://www.tvp.pl/shared/cdn/tokenizer_v2.php?object_id={id}";

        var links = await new Uri(tokenUrl).GetWebFile();
        return GetMesages(links);

    }
    public Task<Result> CanGetMedia(string resourceUrl)
    {
        var result = UrlMatchingRegex.IsMatch(resourceUrl ?? "")
            ? Result.Success()
            : Result.Failure(Resources.Error_MediaNotSupported);
        return Task.FromResult(result);
    }

    private MediaInfo[] GetMesages(string links)
    {
        try
        {
            var result = new List<string>();

            var mediaData = Newtonsoft.Json.JsonConvert.DeserializeObject<TvpMediaData>(links);
            if (mediaData?.formats == null)
            {
                return Array.Empty<MediaInfo>();
            }

            return mediaData?.formats
                ?.OrderBy(o => o.totalBitrate)
                .Select(m => new MediaInfo()
                {
                    Url = new Uri(m.url),
                    FormatDescription = $"Video: {m.totalBitrate}, {m.mimeType}",
                    Title = mediaData.title,
                    OutputFileName = mediaData.title.SanitizeAsFileName(),
                    Extension = m.url.GetMediaExtension()
                })
                .ToArray() ?? Array.Empty<MediaInfo>();
        }
        catch
        {
            return Array.Empty<MediaInfo>();
        }
    }
}
