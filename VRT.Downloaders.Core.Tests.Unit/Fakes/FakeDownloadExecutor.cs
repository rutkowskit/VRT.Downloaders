using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Common.DownloadStates;

namespace VRT.Downloaders.Fakes;

internal sealed class FakeDownloadExecutor : IDownloadExecutorService, IDisposable
{
    private Result _simulatedResult = Result.Success();
    private readonly Dictionary<string, TimeSpan> _delays = new();
    private readonly CancellationTokenSource _cts = new();
    public FakeDownloadExecutor WithSimulatedResult(Result result)
    {
        _simulatedResult = result;        
        return this;
    }

    public FakeDownloadExecutor WithExecutionDelay(TimeSpan delay, string requestName)
    {
        _delays[requestName] = delay;
        return this;
    }

    public Result Cancel()
    {
        _cts.Cancel();
        return _simulatedResult;
    }

    public async Task<Result> Download(IDownloadContext task)
    {
        if(task.Request?.Name is not null && _delays.TryGetValue(task.Request.Name, out var delay))
        {
            await Task.Delay(delay, _cts.Token).ContinueWith(_=> { });
        }
        return _simulatedResult;
    }

    public IDownloadExecutorService WithProgressCallback(Action<int> progressCallback)
    {
        return this;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
