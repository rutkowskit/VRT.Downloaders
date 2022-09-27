using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace VRT.Downloaders
{
    partial class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureCoreServices(this IServiceCollection services)
        {
            return services                
                .AddAppConfig()
                .AddSerilogLogging()
                .AddClientServices()
                .AddViewModels();
        }

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
}
