using CSharpFunctionalExtensions;
using ReactiveUI;
using VRT.Downloaders.Models.Messages;
using VRT.Downloaders.Services.Downloads;

namespace VRT.Downloaders.Maui;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(AppShell shell, IServiceProvider services)
    {
        InitializeComponent();
        MainPage = shell;
        _services = services;
    }
    protected override async void OnStart()
    {
        _ = await EnsurePermissionGranted<Permissions.StorageWrite>()
            .Ensure(EnsurePermissionGranted<Permissions.StorageRead>)
            .Ensure(EnsurePermissionGranted<Permissions.NetworkState>)            
            .Tap(() => StartJobs(_services))
            .OnFailure(() => Environment.Exit(0));

    }
    private static async Task<Result> EnsurePermissionGranted<TPermission>()
        where TPermission : Permissions.BasePermission, new()
    {
        var result = await Permissions.RequestAsync<TPermission>();
        return result == PermissionStatus.Granted
            ? Result.Success()
            : Result.Failure(result.ToString());
    }
    private static void StartJobs(IServiceProvider services)
    {
        _ = services.GetRequiredService<DownloadingWorker>();
    }

    protected override void OnSleep()
    {
        _services.GetService<IMessageBus>()?.SendMessage(new StoreApplicationStateMessage());        
    }
}