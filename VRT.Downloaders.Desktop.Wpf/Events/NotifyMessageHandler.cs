using MediatR;
using System.Threading;
using System.Threading.Tasks;
using VRT.Downloaders.Desktop.Wpf.Views;
using VRT.Downloaders.Models.Messages;

namespace VRT.Downloaders.Maui.Events;
internal class NotifyMessageHandler : INotificationHandler<NotifyMessage>
{
    private readonly MainWindowView _shell;

    public NotifyMessageHandler(MainWindowView shell)
    {
        _shell = shell;
    }
    public async Task Handle(NotifyMessage notification, CancellationToken cancellationToken)
    {
        await _shell.ShowMessage(notification);
    }
}
