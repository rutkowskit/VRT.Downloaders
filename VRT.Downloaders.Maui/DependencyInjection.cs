using VRT.Downloaders.Maui.Pages;

namespace VRT.Downloaders.Maui;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddViews(this IServiceCollection services)
    {
        return services
            .AddSingleton<MainPage>()
            .AddTransient<SettingsPage>(); ;
    }
}
