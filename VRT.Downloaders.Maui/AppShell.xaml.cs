using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ReactiveUI;
using System.Reactive.Linq;
using VRT.Downloaders.Models.Messages;
using VRT.Downloaders.ViewModels;

namespace VRT.Downloaders.Maui
{
    public partial class AppShell : Shell
    {
        private readonly IMessageBus _messageBus;

        public AppShell(MainWindowViewModel viewModel, IMessageBus messageBus)
        {
            InitializeComponent();
            _messageBus = messageBus;
            BindingContext = viewModel;

            _messageBus
                .Listen<NotifyMessage>()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(message => Observable.FromAsync(token => ShowMessage(message)))
                .Concat()
                .Subscribe()
                .Discard();
        }
        private async Task ShowMessage(NotifyMessage message)
        {
            var options = new SnackbarOptions()
            {
                TextColor = message.Type == Properties.Resources.Msg_Success
                    ? Colors.Green
                    : Colors.Red                
            };
            await this.DisplaySnackbar(message.Message, visualOptions: options);
        }
    }
}