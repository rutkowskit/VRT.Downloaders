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
        public AppShell(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));                       
            BindingContext = viewModel;

            MessageBus.Current
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
                    Foreground = message.Type == "Success" ? Color.Green : Color.Red
                }
            };
            await this.DisplayToastAsync(options);
        }
    }
}
