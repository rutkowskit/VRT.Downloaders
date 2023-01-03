using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MediatR;
using VRT.Downloaders.Common.Messages;

namespace VRT.Downloaders.Maui.Events;
internal class NotifyMessageHandler : INotificationHandler<NotifyMessage>
{
    private readonly AppShell _shell;

    public NotifyMessageHandler(AppShell shell)
    {
        _shell = shell;
    }
    public async Task Handle(NotifyMessage notification, CancellationToken cancellationToken)
    {
        var options = new SnackbarOptions()
        {
            TextColor = notification.Type == Properties.Resources.Msg_Success
                    ? Colors.Green
                    : Colors.Red
        };
        await _shell.DisplaySnackbar(notification.Message,             
            actionButtonText: "Close",
            visualOptions: options, 
            token: cancellationToken);
    }
}
