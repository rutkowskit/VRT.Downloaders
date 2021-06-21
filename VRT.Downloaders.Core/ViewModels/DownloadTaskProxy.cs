using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Windows.Input;
using VRT.Downloaders.Services.Downloads;
using VRT.Downloaders.Services.Downloads.DownloadStates;

namespace VRT.Downloaders.ViewModels
{
    public sealed class DownloadTaskProxy : BaseViewModel
    {
        private readonly DownloadTask _task;
        public DownloadTaskProxy(DownloadTask task)
        {
            _task = task;
            Bind(t => t.State, proxy => proxy.State);
            Bind(t => t.DownloadProgress, proxy => proxy.DownloadProgress);
            Bind(t => t.LastErrorMessage, proxy => proxy.LastErrorMessage);
            Bind(t => t.CanRemove, proxy => proxy.CanRemove);
            Bind(t => t.CanCancel, proxy => proxy.CanCancel);

            CancelTaskCommand = ReactiveCommand.Create(_task.Cancel, this.WhenAnyValue(t => t.CanCancel)
                .ObserveOn(RxApp.MainThreadScheduler));
            RemoveTaskCommand = ReactiveCommand.Create(_task.Remove, this.WhenAnyValue(t => t.CanRemove)
                .ObserveOn(RxApp.MainThreadScheduler));
        }

        public string Name => _task.Request.Name;
        public string Uri => _task.Request.Uri.AbsoluteUri;


        [Reactive] public BaseDownloadState.States State { get; private set; }
        [Reactive] public int DownloadProgress { get; private set; }
        [Reactive] public string LastErrorMessage { get; private set; }
        [Reactive] public bool CanRemove { get; private set; }
        [Reactive] public bool CanCancel { get; private set; }

        public ICommand CancelTaskCommand { get; }
        public ICommand RemoveTaskCommand { get; }

        private void Bind<T>(Expression<Func<DownloadTask, T>> taskProperty,
          Expression<Func<DownloadTaskProxy, T>> proxyProperty)
        {
            _task.WhenAnyValue(taskProperty)
                   .ObserveOn(RxApp.MainThreadScheduler)
                   .BindTo(this, proxyProperty)
                   .DisposeWith(Disposables)
                   .Discard();
        }
    }
}
