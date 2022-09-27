using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using System.Windows;
using VRT.Downloaders.Models.Messages;
using VRT.Downloaders.ViewModels;

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

        public MainWindowView(MainWindowViewModel viewModel, IServiceProvider services)
        {
            InitializeComponent();
            _services = services;
            SnackBarQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
            DataContext = viewModel;
        }

        public SnackbarMessageQueue SnackBarQueue
        {
            get => (SnackbarMessageQueue)GetValue(SnackBarQueueProperty);
            set => SetValue(SnackBarQueueProperty, value);
        }

        public Task ShowMessage(NotifyMessage message)
        {
            SnackBarQueue.Enqueue(new SnackbarData(message.Message, message.Type));
            return Task.CompletedTask;
        }

        private void OnOpenSettingsClick(object sender, RoutedEventArgs e)
        {
            var view = _services.GetService<AppSettingsView>();
            view.ShowDialog();
        }
    }
}
