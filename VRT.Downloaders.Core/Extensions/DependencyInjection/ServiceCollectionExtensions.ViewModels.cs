using Microsoft.Extensions.DependencyInjection;
using VRT.Downloaders.ViewModels;

namespace VRT.Downloaders
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            return services
                .AddSingleton<MainWindowViewModel>();
        }
    }
}
