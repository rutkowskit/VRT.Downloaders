using MediatR;
using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Common.Messages;

namespace VRT.Downloaders.Services.DownloadQueue.Events;
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
