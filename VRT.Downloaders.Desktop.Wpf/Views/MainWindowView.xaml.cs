using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using VRT.Downloaders.Models.Messages;
using VRT.Downloaders.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace VRT.Downloaders.Desktop.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window, IActivatableView
    {
        private readonly IServiceProvider _services;

        public sealed record SnackbarData(string Message, string Type)
        {
            public override string ToString() => Message;
        }

        public static readonly DependencyProperty SnackBarQueueProperty
           = DependencyProperty.Register(nameof(SnackBarQueue), typeof(SnackbarMessageQueue), typeof(MainWindowView));

        public static readonly DependencyProperty MessageBusProperty
           = DependencyProperty.Register(nameof(MessageBus), typeof(IMessageBus), typeof(MainWindowView));

        public MainWindowView(MainWindowViewModel viewModel, IMessageBus messageBus,
            IServiceProvider services)
        {
            InitializeComponent();
            _services = services;
            SnackBarQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));

            messageBus.Listen<BringToFrontMessage>()
                .Where(e => e.WindowName == "Main")
                .Subscribe(x => this.BringToFront())
                .Discard();

            messageBus.Listen<NotifyMessage>()
                .Subscribe(x => ShowMessage(x))
                .Discard();
            DataContext = viewModel;
        }

        public SnackbarMessageQueue SnackBarQueue
        {
            get => (SnackbarMessageQueue)GetValue(SnackBarQueueProperty);
            set => SetValue(SnackBarQueueProperty, value);
        }
        public IMessageBus MessageBus
        {
            get => (IMessageBus)GetValue(MessageBusProperty);
            set => SetValue(MessageBusProperty, value);
        }

        private void ShowMessage(NotifyMessage message)
        {
            SnackBarQueue.Enqueue(new SnackbarData(message.Message, message.Type));
        }

        private void OnOpenSettingsClick(object sender, RoutedEventArgs e)
        {            
            var view = _services.GetService<AppSettingsView>();
            //view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            view.ShowDialog();
        }
    }
}
