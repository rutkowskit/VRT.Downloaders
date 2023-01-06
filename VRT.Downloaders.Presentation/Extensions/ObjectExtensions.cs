using System.Reactive.Concurrency;

namespace VRT.Downloaders.Presentation.Extensions;
public static class ObjectExtensions
{
    public static T DoOnDispatcher<T>(this T obj, Action<T> action)
        where T : notnull
    {
        RxApp.MainThreadScheduler.Schedule(() => action(obj));
        return obj;
    }

    public static TResult GetOnDispatcher<T,TResult>(this T obj, Func<T,TResult> valueGetter)
        where T : notnull
    {
        TResult result = default!;
        RxApp.MainThreadScheduler.Schedule(() => result = valueGetter(obj));
        return result;
    }
}
