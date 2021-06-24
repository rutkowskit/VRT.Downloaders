using CSharpFunctionalExtensions;
using System.Threading.Tasks;

namespace VRT.Downloaders.Services.Downloads.DownloadStates
{
    internal sealed class FakeDownloadExecutor : IDownloadExecutor
    {
        private Result _simulatedResult = Result.Success();
        
        public FakeDownloadExecutor WithSimulatedResult(Result result)
        {
            _simulatedResult = result;
            return this;
        }

        public Result Cancel()
        {
            return _simulatedResult;
        }

        public Task<Result> Download(IDownloadContext task)
        {
            return Task.FromResult(_simulatedResult);
        }
    }
}
