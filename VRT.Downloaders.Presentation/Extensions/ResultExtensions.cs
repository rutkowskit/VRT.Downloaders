namespace VRT.Downloaders.Presentation.Extensions;
internal static class ResultExtensions
{
    public static async Task<Result<T>> TapOnDispatcher<T>(this Task<Result<T>> resultTask, Action<T> action)
    {
        return await resultTask.Tap(r => action(r));
    }
}
