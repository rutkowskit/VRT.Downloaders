using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VRT.Downloaders.Desktop.Wpf.Views;
using VRT.Downloaders.Models.Messages;

namespace VRT.Downloaders.Desktop.Wpf.Events;
internal sealed class BringToFrontMessageHandler
    : INotificationHandler<BringToFrontMessage>
{
    public BringToFrontMessageHandler(MainWindowView mainView)
    {
        MainView = mainView;
    }

    public MainWindowView MainView { get; }

    public Task Handle(BringToFrontMessage notification, CancellationToken cancellationToken)
    {
        if (notification.WindowName == "Main")
        {
            MainView.BringToFront();
        }
        return Task.CompletedTask;
    }
}
