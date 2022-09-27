using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VRT.Downloaders.Services.AppStates;
using VRT.Downloaders.Services.Confirmation;
using VRT.Downloaders.Services.FileSystem;
using VRT.Downloaders.Services.Medias.TvpVod;
using VRT.Downloaders.Services.Medias.Youtube;

namespace VRT.Downloaders
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClientServices(this IServiceCollection services)
        {
            return services
                .WhenNotExists<IFileSystemService>(s => s.AddSingleton<IFileSystemService, DefaultFilesystemService>())
                .AddSingleton<IDownloadQueueService, DownloadQueueService>()
                .AddSingleton<DownloadingWorker>()
                .AddSingleton<IMediaService, YoutubeMediaService>()
                .AddSingleton<IMediaService, TvpVodMediaService>()
                .AddSingleton<IAppStateService, DefaultAppStateService>()
                .AddSingleton<IConfirmationService, AlwaysTrueConfirmationService>()
                .AddMediatR(typeof(ServiceCollectionExtensions).Assembly);
        }
    }
}
