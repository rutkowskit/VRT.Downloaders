using MediatR;
using System.Threading;

namespace VRT.Downloaders.Services.Downloads.Events;
internal class StoreApplicationStateMessageHandler : INotificationHandler<StoreApplicationStateMessage>
{
    private readonly IDownloadQueueService _downloadQueueService;

    public StoreApplicationStateMessageHandler(IDownloadQueueService downloadQueueService)
    {
        _downloadQueueService = downloadQueueService;
    }

    public async Task Handle(StoreApplicationStateMessage notification, CancellationToken cancellationToken)
    {
        await _downloadQueueService.StoreDownloadQueueState();
    }
}
