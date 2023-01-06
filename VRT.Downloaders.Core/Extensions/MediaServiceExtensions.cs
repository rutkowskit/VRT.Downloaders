using VRT.Downloaders.Services.Medias;

namespace VRT.Downloaders.Extensions;
public static class MediaServiceExtensions
{
    public static async Task<Result<IMediaService>> GetMediaService(
        this IEnumerable<IMediaService> mediaServices,
        string url)
    {
        Guard.AgainstNull(mediaServices);
        if (string.IsNullOrWhiteSpace(url))
        {
            return Result.Failure<IMediaService>(Resources.Error_NotSupported);
        }

        foreach (var service in mediaServices)
        {
            var canGetMedia = await service.CanGetMedia(url);
            if (canGetMedia.IsSuccess)
                return Result.Success(service);
        }
        return Result.Success<IMediaService>(new DirectMediaService());
    }
}
