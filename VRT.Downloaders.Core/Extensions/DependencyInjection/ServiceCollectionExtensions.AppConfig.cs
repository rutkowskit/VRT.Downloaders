using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using VRT.Downloaders.Services.Configs;
using VRT.Downloaders.Services.FileSystem;

namespace VRT.Downloaders
{
    partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppConfig(this IServiceCollection services)
        {
            return services
                .AddSingleton(provider => InitConfiguration(provider))
                .AddSingleton<IAppSettingsService, DefaultAppSettingsService>();
        }
        private static IConfiguration InitConfiguration(IServiceProvider provider)
        {
            var fsService = provider.GetRequiredService<IFileSystemService>();

            return new ConfigurationBuilder()
             .SetBasePath(fsService.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             .Build();
        }
    }
}
