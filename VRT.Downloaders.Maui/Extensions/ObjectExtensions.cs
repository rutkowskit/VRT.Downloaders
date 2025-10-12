using System.Reactive.Concurrency;

namespace VRT.Downloaders.Maui.Extensions;
public static class ObjectExtensions
{
    public static async Task<T> DoOnDispatcher<T>(this T obj, Action<T> action)
        where T : notnull
    {
        Task asyncTask(T o)
        {
            action(o);
            return Task.CompletedTask;
        }
        await obj.DoOnDispatcher(asyncTask);
        return obj;
    }

    public static async Task<T> DoOnDispatcher<T>(this T obj, Func<T, Task> action)
        where T : notnull
    {
        if (MainThread.IsMainThread)
        {
            await action(obj);
        }
        else
        {
            await MainThread.InvokeOnMainThreadAsync(() => action(obj));
        }
        //RxApp.MainThreadScheduler.Schedule(() => action(obj));
        return obj;
    }

    public static TResult GetOnDispatcher<T, TResult>(this T obj, Func<T, TResult> valueGetter)
        where T : notnull
    {
        TResult result = default!;
        RxApp.MainThreadScheduler.Schedule(() => result = valueGetter(obj));
        return result;
    }
}
