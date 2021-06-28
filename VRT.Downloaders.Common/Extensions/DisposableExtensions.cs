using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace VRT.Downloaders
{
    public static class DisposableExtensions
    {
        public static IDisposable DisposeWith(this IDisposable disposable,
            CompositeDisposable disposables)
        {
            disposables.Add(disposable);
            return disposable;
        }

        public static TDisposable SetDisposable<TDisposable>(this TDisposable disposable,
            CompositeDisposable disposables)
            where TDisposable : IDisposable
        {
            disposable.DisposeWith(disposables).Discard();
            return disposable;
        }

        public async static Task<TDisposable> DisposeWith<TDisposable>(this Task<TDisposable> disposable,
            CompositeDisposable disposables)
            where TDisposable : IDisposable
        {
            var toDispose = await disposable;
            ((IDisposable)toDispose).DisposeWith(disposables).Discard();
            return toDispose;
        }
    }
}
