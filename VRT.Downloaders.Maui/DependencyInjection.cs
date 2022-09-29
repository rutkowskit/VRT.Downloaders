using VRT.Downloaders.Maui.Pages;
using VRT.Downloaders.Maui.Services;
using VRT.Downloaders.Services.Confirmation;
using VRT.Downloaders.Services.FileSystem;

namespace VRT.Downloaders.Maui;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        return services
            .AddTransient<IFolderPickerService, FolderPickerService>()
            .AddSingleton<AppShell>()
            .AddTransient<IConfirmationService>(p => p.GetRequiredService<AppShell>())
            .AddSingleton<MainPage>()
            .AddTransient<SettingsPage>();            
    }
}
