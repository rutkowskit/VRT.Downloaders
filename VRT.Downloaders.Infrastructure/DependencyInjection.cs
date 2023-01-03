using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using VRT.Downloaders.Abstractions.FileSystem;
using VRT.Downloaders.Common.Options;
using VRT.Downloaders.Extensions;
using VRT.Downloaders.Infrastructure.AppStates;
using VRT.Downloaders.Infrastructure.Configuration;
using VRT.Downloaders.Infrastructure.DownloadExecutor;
using VRT.Downloaders.Infrastructure.Options;
using VRT.Downloaders.Services.Confirmation;
using VRT.Downloaders.Services.Medias;
using VRT.Downloaders.Services.Medias.TvpVod;
using VRT.Downloaders.Services.Medias.Youtube;

namespace VRT.Downloaders.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddSerilogLogging()
            .AddAppConfig()
            .ConfigureOptions<DownloadingWorkerOptionsSetup>()
            .WhenNotExists<IFileSystemService>(s => s.AddSingleton<IFileSystemService, DefaultFileSystemService>())
            .AddSingleton<IMediaService, YoutubeMediaService>()
            .AddSingleton<IMediaService, TvpVodMediaService>()
            .AddSingleton<IAppStateService, AppStateService>()
            .AddSingleton<IConfirmationService, AlwaysTrueConfirmationService>()
            .AddSingleton<IDownloadExecutorService, DownloadExecutorService>()
            .AddMediatR(typeof(DependencyInjection).Assembly);
    }

    public static IServiceCollection AddAppConfig(this IServiceCollection services)
    {
        return services
            .AddSingleton(InitConfiguration)
            .AddSingleton<IAppSettingsService, AppSettingsService>();
    }
    private static IConfiguration InitConfiguration(IServiceProvider provider)
    {
        var fsService = provider.GetRequiredService<IFileSystemService>();

        return new ConfigurationBuilder()
         .SetBasePath(fsService.GetCurrentDirectory())
         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
         .Build();
    }

    public static IServiceCollection AddSerilogLogging(this IServiceCollection services)
            => services.AddLogging(builder => builder.AddSerilogFromConfig());

    private static ILoggingBuilder AddSerilogFromConfig(this ILoggingBuilder builder)
    {
        builder.AgainstNull(nameof(builder)).Discard();
        builder.Services.AddSingleton<ILoggerProvider, SerilogLoggerProvider>(services =>
        {
            var config = services.GetRequiredService<IConfiguration>();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();
            return new SerilogLoggerProvider(Log.Logger, true);
        });

        builder.AddFilter<SerilogLoggerProvider>(null, LogLevel.Trace);
        return builder;
    }
}
