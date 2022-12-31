﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace VRT.Downloaders;

partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddSerilogLogging(this IServiceCollection services)
        => services.AddLogging(builder => builder.AddSerilogFromConfig());

    private static ILoggingBuilder AddSerilogFromConfig(this ILoggingBuilder builder)
    {
        Guard.AgainstNull(builder, nameof(builder)).Discard();
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
