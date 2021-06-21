using Microsoft.Extensions.DependencyInjection;

namespace VRT.Downloaders.Mobile
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            return services
                .AddSingleton<AppShell>();
        }
    }
}
