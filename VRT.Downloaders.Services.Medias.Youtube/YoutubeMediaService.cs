using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;
using VRT.Downloaders.Services.Medias.Properties;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace VRT.Downloaders.Services.Medias.Youtube
{
    public sealed class YoutubeMediaService : IMediaService
    {
        private static readonly Regex UrlMatchingRegex = new Regex(
            @"^http[s]?://(?:(?:www|m)\.)?(?:youtube\..{2,3}|youtu\.be)\/.{5,}$",
            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        public async Task<Result<MediaInfo[]>> GetAvailableMedias(string resourceUrl)
        {
            var uri = new Uri(resourceUrl);
            var result = await Result
                .Success(new YoutubeClient())
                .Map(async grabber =>
                {
                    var videoInfo = await grabber.Videos.GetAsync(resourceUrl);
                    var streams = await grabber.Videos.Streams.GetManifestAsync(resourceUrl);
                    return ToMediaInfo(videoInfo, streams);
                });
            return result;
        }
        public Task<Result> CanGetMedia(string resourceUrl)
        {
            var result = UrlMatchingRegex.IsMatch(resourceUrl ?? "")
                ? Result.Success()
                : Result.Failure(Resources.Error_MediaNotSupported);
            return Task.FromResult(result);
        }
        private static Result<MediaInfo[]> ToMediaInfo(Video videoInfo, StreamManifest streams)
        {
            var results = new List<MediaInfo>();
            results.AddRange(GetAudioStreams(videoInfo, streams));
            results.AddRange(GetMuxedStreams(videoInfo, streams));
            return results.ToArray();
        }
        private static MediaInfo[] GetAudioStreams(Video videoInfo, StreamManifest manifest)
        {
            return manifest
                .GetAudioOnlyStreams()
                .Where(IsOriginalLanguage)
                .OrderByDescending(s => s.Bitrate)
                .Select(a => new MediaInfo()
                {
                    Url = new Uri(a.Url),
                    Title = videoInfo.Title,
                    Extension = a.Container.Name,
                    FormatDescription = $"Audio: {a.AudioCodec}{a.AudioLanguage?.Name}({a.Container.Name})",
                    OutputFileName = videoInfo.Title.SanitizeAsFileName()
                })
                .ToArray();
        }
        private static bool IsOriginalLanguage(IAudioStreamInfo stream)
        {
            return stream.AudioLanguage?
                .Name.Contains("original", StringComparison.InvariantCultureIgnoreCase)
                ?? false;
        }
        private static MediaInfo[] GetMuxedStreams(Video videoInfo, StreamManifest manifest)
        {
            return manifest
                .GetVideoOnlyStreams()
                .OrderByDescending(s => s.VideoResolution.Width)
                .Select(a => new MediaInfo()
                {
                    Url = new Uri(a.Url),
                    Title = videoInfo.Title,
                    Extension = a.Container.Name,
                    FormatDescription = $"Video: {a.Container.Name} {a.VideoQuality} {a.VideoResolution} {a.VideoCodec}",
                    OutputFileName = videoInfo.Title.SanitizeAsFileName()
                })
                .ToArray();
        }
    }
}
