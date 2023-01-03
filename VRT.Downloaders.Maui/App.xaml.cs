using CSharpFunctionalExtensions;
using MediatR;
using VRT.Downloaders.Common.Messages;
using VRT.Downloaders.Workers;

namespace VRT.Downloaders.Maui;

public partial class App : Application
{
    private readonly IServiceProvider _services;
    private readonly IMediator _mediator;

    public App(AppShell shell, IServiceProvider services, IMediator mediator)
    {
        InitializeComponent();
        MainPage = shell;
        _services = services;
        _mediator = mediator;
    }
    protected override async void OnStart()
    {
        _ = await EnsurePermissionGranted<Permissions.StorageWrite>()
            .Ensure(EnsurePermissionGranted<Permissions.StorageRead>)
            .Ensure(EnsurePermissionGranted<Permissions.NetworkState>)
            .Tap(() => StartJobs(_services))
            .TapError(() => Environment.Exit(0));
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

    protected override async void OnSleep()
    {
        await _mediator.Publish(new StoreApplicationStateMessage());
    }
}