using CSharpFunctionalExtensions;

namespace VRT.Downloaders.Services.Medias;

public interface IMediaService
{
    Task<Result<MediaInfo[]>> GetAvailableMedias(string resourceUrl);
    Task<Result> CanGetMedia(string resourceUrl);
    int GetServicePriority() => 0;
}
