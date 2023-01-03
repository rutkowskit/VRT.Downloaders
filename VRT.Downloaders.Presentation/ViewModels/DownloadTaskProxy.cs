using CommunityToolkit.Mvvm.Input;
using System.Reactive.Concurrency;
using VRT.Downloaders.Presentation.Extensions;

namespace VRT.Downloaders.Presentation.ViewModels;

public sealed partial class DownloadTaskProxy : BaseViewModel
{
    private readonly DownloadTask _task;
    public DownloadTaskProxy(DownloadTask task)
    {
        _task = task;
        _task.StateChanged += OnTaskStateChanged;        
        _task.PropertyChanged += OnTaskPropertyChanged;        
    }

    private void OnTaskStateChanged(object? sender, BaseDownloadState e)
    {
        this.DoOnDispatcher(p =>
        {
            p.CancelTaskCommand.NotifyCanExecuteChanged();
            p.RemoveTaskCommand.NotifyCanExecuteChanged();
            p.RetryTaskCommand.NotifyCanExecuteChanged();
        });        
    }

    public string Name => _task.Request.Name;
    public string Uri => _task.Request.Uri.AbsoluteUri;
    public BaseDownloadState.States State => _task.State;
    public int DownloadProgress => _task.DownloadProgress;
    public string? LastErrorMessage => _task.LastErrorMessage;
    public bool CanCancel => _task.CanCancel;
    public bool CanRemove => _task.CanRemove;


    [RelayCommand(CanExecute = nameof(CanCancelTask))]
    private async Task CancelTask() => await _task.Cancel();
    private bool CanCancelTask() => _task.CanCancel;

    [RelayCommand(CanExecute = nameof(CanRemoveTask))]
    private async Task RemoveTask() => await _task.Remove();
    private bool CanRemoveTask() => _task.CanRemove;

    [RelayCommand(CanExecute = nameof(CanRemoveTask))]
    private async Task RetryTask() => await _task.Download();

    private void OnTaskPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _task.StateChanged -= OnTaskStateChanged;
            _task.PropertyChanged -= OnTaskPropertyChanged;
        }
        base.Dispose(disposing);
    }
}
