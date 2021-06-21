using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace VRT.Downloaders.Services.Medias
{
    public interface IMediaService
    {
        Task<Result<MediaInfo[]>> GetAvailableMedias(string resourceUrl);
    }
}
