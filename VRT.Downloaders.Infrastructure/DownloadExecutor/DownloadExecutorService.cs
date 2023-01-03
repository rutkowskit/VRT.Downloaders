using VRT.Downloaders.Common.Collections;
using VRT.Downloaders.Common.DownloadStates;
using VRT.Downloaders.Properties;

namespace VRT.Downloaders.Infrastructure.DownloadExecutor;

public sealed class DownloadExecutorService : IDisposable, IDownloadExecutorService
{
    private int _isDownloading;
    private CancellationTokenSource? _tokenSource;
    private Action<int>? _progressCallback;

    public IDownloadExecutorService WithProgressCallback(Action<int> progressCallback)
    {
        _progressCallback = progressCallback;
        return this;
    }

    public Result Cancel()
    {
        if (_tokenSource == null)
        {
            return Result.Failure(Resources.Error_CannotCancelTask);
        }
        _tokenSource.Cancel();
        return Result.Success();
    }

    public async Task<Result> Download(IDownloadContext task)
    {
        if (Interlocked.CompareExchange(ref _isDownloading, 1, 0) > 0)
        {
            return Result.Failure(Resources.Info_DownloadInProgress);
        }

        try
        {
            CreateDirectoryFor(task.Request.OutputFileName);            
            using (_tokenSource = new CancellationTokenSource())
            {
                var result = await CheckExistingPartitions(task)
                    .OnFailureCompensate(() => CreatePartitions(task))
                    .Bind(context => Download(context, _progressCallback, _tokenSource.Token));
                return result;
            }
            //.OnFailure(() => File.Delete(task.Request.OutputFileName));            
        }
        finally
        {
            Interlocked.Exchange(ref _isDownloading, 0).Discard();
            _tokenSource = null;
        }
    }

    private static async Task<Result<IDownloadContext>> CreatePartitions(IDownloadContext task)
    {
        return await task.Request.Uri.CreatePartitions()
            .Tap(p => task.Partitions = p)
            .Map(_ => task);
    }

    private static Result<IDownloadContext> CheckExistingPartitions(IDownloadContext context)
    {
        var ranges = context.Partitions?.Partitions ?? Array.Empty<FileByteRange>();
        return ranges.Length > 0
            ? Result.Success(context)
            : Result.Failure<IDownloadContext>(Resources.Error_PartitionsNotCreated);
    }
    private static void CreateDirectoryFor(string outputFile)
    {
        var directory = Path.GetDirectoryName(outputFile);
        if (string.IsNullOrWhiteSpace(directory) is false)
        {
            Directory.CreateDirectory(directory).Discard();
        }
    }

    private static async Task<Result> Download(
        IDownloadContext downloadContext,
        Action<int>? progressCallback,
        CancellationToken cancellationToken)
    {
        var outputFileName = downloadContext.Request.OutputFileName;
        var partitions = downloadContext.Partitions.AgainstNull();
        using var context = new DownloadToFileInputOutputContext(outputFileName, cancellationToken: cancellationToken);
        var parts = partitions.AgainstNull();
        var downloadedSum = downloadContext.Partitions!.DownloadedSize;
        var result = await partitions!.Partitions
            .Where(range => !range.IsDownloaded)
            .Select(range => DoDownload(partitions.ResourcePath, range, context)) //Here
            .ToList()
            .DoParallel(downloaded =>
            {
                downloaded.IsDownloaded = true;
                downloaded.LastError = null;
                var sum = Interlocked.Add(ref downloadedSum, downloaded.Size);
                var progress = (int)((double)sum / partitions.FileSize * 100);
                progressCallback?.Invoke(progress % 101);
            }, cancellationToken: cancellationToken);
        return result;
    }

    private static async Task<Result<FileByteRange>> DoDownload(Uri url, FileByteRange range, DownloadToFileInputOutputContext context)
    {
        return await WhileSuccess(async () => await DownloadRange(url, range, context),
            () => context.CancellationToken.IsCancellationRequested, -1);
    }

    private static async Task<Result<FileByteRange>> DownloadRange(Uri url, FileByteRange range, DownloadToFileInputOutputContext context)
    {
        await context.ReadSemaphore.WaitAsync(context.CancellationToken);
        try
        {
            using (var remoteStream = new RemoteStream(url, range))
            using (var stream = await remoteStream.Open())
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return Result.Failure<FileByteRange>(Resources.Info_DownloadCanceled);
                }
                await stream.CopyTo(context.OutputStream, context.WriteSemaphore, range.From);
            }
            return Result.Success(range);
        }
        catch (Exception e)
        {
            range.LastError = e.ToString();
            return Result.Failure<FileByteRange>(e.Message);
        }
        finally
        {
            context.ReadSemaphore.Release().Discard();
        }
    }

    private static async Task<Result<T>> WhileSuccess<T>(Func<Task<Result<T>>> doAction,
        Func<bool> cancelationRequested, int numberOfRetries = 3)
    {
        var numberOfTries = 0;
        Result<T> result;
        do
        {
            if (cancelationRequested())
            {
                return Result.Failure<T>(Resources.Info_DownloadCanceled);
            }
            result = await doAction();
            if (result.IsSuccess)
            {
                break;
            }
        } while (numberOfRetries == -1 || numberOfTries++ <= numberOfRetries);
        return result;
    }

    public void Dispose()
    {
        _tokenSource?.Dispose();
        _tokenSource = null;
    }
}
