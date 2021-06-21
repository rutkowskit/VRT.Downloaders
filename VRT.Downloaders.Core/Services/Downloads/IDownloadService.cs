using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace VRT.Downloaders.Services.Downloads
{
    public interface IDownloadService
    {
        Task<Result> PerformDownload(DownloadTask task);
    }
}
