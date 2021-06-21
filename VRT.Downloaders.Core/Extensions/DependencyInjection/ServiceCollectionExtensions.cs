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
    }
}
