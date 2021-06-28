using ReactiveUI;
using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Threading.Tasks;
using VRT.Downloaders.Models.Messages;
using VRT.Downloaders.ViewModels;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;

namespace VRT.Downloaders.Mobile
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        private readonly IMessageBus _messageBus;

        public AppShell(MainWindowViewModel viewModel, IMessageBus messageBus)
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));                       
            _messageBus = messageBus;
            BindingContext = viewModel;

            _messageBus
                .Listen<NotifyMessage>()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async message => await ShowMessage(message))
                .Discard();
        }

        private async Task ShowMessage(NotifyMessage message)
        {
            var options = new SnackBarOptions()
            {
                MessageOptions = new MessageOptions()
                {
                    Message = message.Message,
                    Foreground = message.Type == Properties.Resources.Msg_Success 
                        ? Color.Green
                        : Color.Red
                }
            };
            await this.DisplayToastAsync(options);
        }
    }
}
