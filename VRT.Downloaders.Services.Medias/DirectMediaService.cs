using CSharpFunctionalExtensions;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using VRT.Downloaders.Services.Medias.Properties;

namespace VRT.Downloaders.Services.Medias
{
    public sealed class DirectMediaService : IMediaService
    {
        public async Task<Result<MediaInfo[]>> GetAvailableMedias(string resourceUrl)
        {
            if (!Uri.TryCreate(resourceUrl, UriKind.Absolute, out var parsedUrl))
            {
                return Result.Failure<MediaInfo[]>("Invalid Url");
            }

            await Task.Yield();
            var extension = resourceUrl.GetMediaExtension();
            var mediaInfo = new MediaInfo()
            {
                Url = parsedUrl,
                FormatDescription = string.IsNullOrWhiteSpace(extension)
                        ? "unknown"
                        : extension,
                Extension = extension,
                Title = HttpUtility.UrlDecode(Path.GetFileName(resourceUrl))
            };
            return new[] { mediaInfo };
        }
        public Task<Result> CanGetMedia(string resourceUrl)
        {
            // always return false to avoid breaking the chain before checking other media services
            return Task.FromResult(Result.Failure(Resources.Error_MediaNotSupported)); 
        }
    }
}
