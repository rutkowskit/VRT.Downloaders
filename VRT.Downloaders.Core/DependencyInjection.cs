using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Services.DownloadQueue;
using VRT.Downloaders.Workers;

namespace VRT.Downloaders;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDownloadQueueService, DownloadQueueService>()
            .AddSingleton<DownloadingWorker>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly));
    }
}
