namespace VRT.Downloaders.Presentation.Extensions;
public static class ResultExtensions
{
    public static async Task<Result<T>> TapOnDispatcher<T>(this Task<Result<T>> resultTask, Action<T> action)
        where T : notnull
    {
        return await resultTask.Tap(r => r.DoOnDispatcher(action));
    }
}
