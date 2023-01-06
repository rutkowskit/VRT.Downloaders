using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using VRT.Downloaders.Common.Messages;

namespace VRT.Downloaders.Maui.Events;
internal class NotifyMessageHandler : INotificationHandler<NotifyMessage>
{
    private readonly AppShell _shell;
    private readonly ILogger<NotifyMessageHandler> _logger;

    public NotifyMessageHandler(AppShell shell, ILogger<NotifyMessageHandler> logger)
    {
        _shell = shell;
        _logger = logger;
    }
    public Task Handle(NotifyMessage notification, CancellationToken cancellationToken)
    {
        var options = new SnackbarOptions()
        {
            TextColor = notification.Type == Properties.Resources.Msg_Success
                ? Colors.Green
                : Colors.Red
        };
        try
        {
            _ = _shell.DisplaySnackbar(notification.Message,
                actionButtonText: "Close",
                visualOptions: options,
                token: cancellationToken);
        }
        catch(Exception ex) 
        {
            _logger?.LogCritical(ex, "{Message}", ex.Message);
        }        
        return Task.CompletedTask;
    }
}
