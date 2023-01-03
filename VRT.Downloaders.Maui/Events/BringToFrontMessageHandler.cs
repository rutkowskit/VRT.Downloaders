using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRT.Downloaders.Common.Messages;
using VRT.Downloaders.Maui.Helpers;

namespace VRT.Downloaders.Maui.Events;
internal sealed class BringToFrontMessageHandler
    : INotificationHandler<BringToFrontMessage>
{
    public Task Handle(BringToFrontMessage notification, CancellationToken cancellationToken)
    {
        if(notification.WindowName == "Main")
        {
            WindowHelpers.BringToFront();         
        }
        return Task.CompletedTask;
    }
}
