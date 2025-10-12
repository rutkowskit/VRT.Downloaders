using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Maui.Pages;
using VRT.Downloaders.Maui.Services;
using VRT.Downloaders.Maui.ViewModels;

namespace VRT.Downloaders.Maui;

public static partial class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        return services
            .AddViewModels()
            .AddTransient<IFolderPickerService, FolderPickerService>()
            .AddSingleton<AppShell>()
            .AddSingleton<IConfirmationService>(p => p.GetService<AppShell>()!)
            .AddSingleton<MainPage>()
            .AddTransient<SettingsPage>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MauiProgram).Assembly));
    }

    private static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        return services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<SettingsViewModel>();
    }
}
