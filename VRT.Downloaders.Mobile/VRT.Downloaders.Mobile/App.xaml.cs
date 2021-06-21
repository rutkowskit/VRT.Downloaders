using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using VRT.Downloaders.Services.Downloads;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VRT.Downloaders.Mobile
{
    public partial class App : Application
    {
        public static IServiceProvider Services => ((App)Current).ServiceProvider;
        public IServiceProvider ServiceProvider { get; }

        public App(Action<IServiceCollection> onConfig)
        {
            InitializeComponent();
            ServiceProvider = ConfigureServiceProvider(onConfig, ConfigureServices);
            MainPage = ServiceProvider.GetRequiredService<AppShell>();
        }

        protected override async void OnStart()
        {
            await EnsurePermissionGranted<Permissions.StorageWrite>()
                .Bind(() => EnsurePermissionGranted<Permissions.StorageRead>())                
                .Tap(() => StartJobs(ServiceProvider))
                .OnFailure(() => System.Environment.Exit(0));            
            
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

        private async Task<Result> EnsurePermissionGranted<TPermission>()
            where TPermission : Permissions.BasePermission, new()
        {
            var result = await Permissions.RequestAsync<TPermission>();
            return result == PermissionStatus.Granted
                ? Result.Success()
                : Result.Failure(result.ToString());
        }

        private static IServiceProvider ConfigureServiceProvider(params Action<IServiceCollection>[] setupActions)
        {
            var serviceCollection = new ServiceCollection();            
            foreach (var setup in setupActions)
            {
                if (setup != null)
                {
                    setup(serviceCollection);
                }
            }
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
