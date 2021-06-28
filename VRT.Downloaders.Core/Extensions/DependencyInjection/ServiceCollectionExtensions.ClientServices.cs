using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using VRT.Downloaders.Services.AppStates;
using VRT.Downloaders.Services.Downloads;
using VRT.Downloaders.Services.FileSystem;
using VRT.Downloaders.Services.Medias;
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
                .AddSingleton<IMessageBus, MessageBus>()
                .AddSingleton<IAppStateService, DefaultAppStateService>();
        }
    }
}
