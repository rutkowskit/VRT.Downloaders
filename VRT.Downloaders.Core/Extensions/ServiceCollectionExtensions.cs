using Microsoft.Extensions.DependencyInjection;

namespace VRT.Downloaders.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection WhenNotExists<TService>(this IServiceCollection services,
        Action<IServiceCollection> setupAction)
    {
        foreach (var service in services)
        {
            if (service.ServiceType == typeof(TService))
            {
                return services;
            }
        }
        setupAction(services);
        return services;
    }
}
