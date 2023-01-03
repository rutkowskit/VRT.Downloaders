using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Common.DownloadStates;

namespace VRT.Downloaders.Fakes;

internal sealed class FakeDownloadExecutor : IDownloadExecutorService
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

    public IDownloadExecutorService WithProgressCallback(Action<int> progressCallback)
    {
        return this;
    }
}
