using Microsoft.Extensions.DependencyInjection;
using VRT.Downloaders.Presentation.ViewModels;

namespace VRT.Downloaders.Presentation;
public static class DependencyInjection
{
    public static IServiceCollection AddPresentationCore(this IServiceCollection services)
    {
        return services
            .AddMediatR(typeof(DependencyInjection).Assembly)
            .AddViewModels();
    }

    private static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        return services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<SettingsViewModel>();
    }
}
