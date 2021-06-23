using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;
using System.Windows.Threading;
using VRT.Downloaders.Desktop.Wpf.Views;
using VRT.Downloaders.Services.Downloads;

namespace VRT.Downloaders.Desktop.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _appHost;
        public App()
        {
            ShutdownMode = ShutdownMode.OnLastWindowClose;
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;

            InitializeHost();
        }
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = _appHost.Services.GetService<ILogger<App>>();
            logger?.LogCritical(e.Exception.ToString());
        }

        private void InitializeHost()
        {
            _appHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => ConfigureServices(services))
                .Build();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .ConfigureCoreServices()
                .AddViews()
                .Discard();
        }

        private static void StartJobs(IServiceProvider services)
        {
            _ = services.GetRequiredService<DownloadingWorker>();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            await _appHost.StartAsync();
            StartJobs(_appHost.Services);
            ShowMainWindow(_appHost.Services);            
            base.OnStartup(e);
        }
        protected override async void OnExit(ExitEventArgs e)
        {
            using (_appHost)
            {
                await _appHost.StopAsync();
            }
            base.OnExit(e);
        }
        private static void ShowMainWindow(IServiceProvider services)
        {
            services.GetRequiredService<MainWindowView>().Show();
        }
    }
}
