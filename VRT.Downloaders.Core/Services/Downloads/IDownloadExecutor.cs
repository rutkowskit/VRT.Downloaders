using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using VRT.Downloaders.Services.Downloads.DownloadStates;

namespace VRT.Downloaders.Services.Downloads
{
    public interface IDownloadExecutor
    {
        Result Cancel();
        Task<Result> Download(IDownloadContext task);
    }
}