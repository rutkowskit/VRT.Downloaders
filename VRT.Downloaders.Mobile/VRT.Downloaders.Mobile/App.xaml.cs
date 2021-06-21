using Microsoft.Extensions.DependencyInjection;
using System;
using VRT.Downloaders.Services.Downloads;
using Xamarin.Forms;

namespace VRT.Downloaders.Mobile
{
    public partial class App : Application
    {
        public static IServiceProvider Services => ((App)Current).ServiceProvider;
        public IServiceProvider ServiceProvider { get; }

        public App()
        {
            InitializeComponent();
            ServiceProvider = ConfigureServiceProvider();
            MainPage = ServiceProvider.GetRequiredService<AppShell>();
        }

        protected override void OnStart()
        {
            StartJobs(ServiceProvider);
        }
        private static void StartJobs(IServiceProvider services)
        {
            _ = services.GetRequiredService<DownloadingWorker>();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private static IServiceProvider ConfigureServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .ConfigureCoreServices()
                .AddViews()
                .Discard();
        }
    }
}
