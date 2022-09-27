using VRT.Downloaders.Maui.Pages;
using VRT.Downloaders.Services.Confirmation;

namespace VRT.Downloaders.Maui;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        return services
            .AddSingleton<AppShell>()
            .AddTransient<IConfirmationService>(p => p.GetRequiredService<AppShell>())
            .AddSingleton<MainPage>()
            .AddTransient<SettingsPage>();            
    }
}
