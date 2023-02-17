using MediatR;
using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Maui.Pages;
using VRT.Downloaders.Maui.Services;

namespace VRT.Downloaders.Maui.Extensions;

public static partial class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        return services
            .AddTransient<IFolderPickerService, FolderPickerService>()
            .AddSingleton<AppShell>()
            .AddSingleton<IConfirmationService>(p => p.GetService<AppShell>()!)
            .AddSingleton<MainPage>()
            .AddTransient<SettingsPage>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MauiProgram).Assembly));
    }
}
