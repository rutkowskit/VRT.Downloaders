namespace VRT.Downloaders.Infrastructure.DownloadExecutor;

public static class StreamExtensions
{
    public static async Task CopyTo(this Stream sourceStream, Stream outputStream,
       SemaphoreSlim writeSemaphore, long outputStreamPosition)
    {
        await writeSemaphore.WaitAsync();
        try
        {
            outputStream.Position = outputStreamPosition;
            await sourceStream.CopyToAsync(outputStream, 1024);
        }
        finally
        {
            writeSemaphore.Release().Discard();
        }
    }
}
