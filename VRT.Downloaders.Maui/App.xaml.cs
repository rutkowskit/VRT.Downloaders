using CSharpFunctionalExtensions;
using MediatR;
using VRT.Downloaders.Common.Messages;
using VRT.Downloaders.Workers;

namespace VRT.Downloaders.Maui;

public partial class App : Application
{
    private readonly IServiceProvider _services;
    private readonly IMediator _mediator;
    private readonly AppShell _shell;

    public App(AppShell shell, IServiceProvider services, IMediator mediator)
    {
        InitializeComponent();
        _shell = shell;
        _services = services;
        _mediator = mediator;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new(_shell);
    }

    protected override async void OnStart()
    {
        _ = await EnsurePermissionGranted<Permissions.StorageWrite>()
            .Ensure(EnsurePermissionGranted<Permissions.StorageRead>)
            .Ensure(EnsurePermissionGranted<Permissions.NetworkState>)
            .Tap(() => StartJobs(_services))
            .TapError(() => Environment.Exit(0));
        var mainWindow = GetMainWindow();
        if (mainWindow is not null)
        {
            mainWindow.Destroying += OnDestroying;
        }
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

    private async void OnDestroying(object? sender, EventArgs e)
    {
        var mainWindow = GetMainWindow();
        if (mainWindow is not null)
        {
            mainWindow.Destroying -= OnDestroying;
        }
        await _mediator.Publish(new StoreApplicationStateMessage());
    }
    private Window? GetMainWindow()
    {
        return Windows switch
        {
            { Count: 0 } => null,
            [Window window, ..] => window,
            _ => null
        };
    }
}