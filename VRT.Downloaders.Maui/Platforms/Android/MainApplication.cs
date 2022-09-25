using Android.App;
using Android.Runtime;
using VRT.Downloaders.Maui.Platforms.Android.Services;
using VRT.Downloaders.Services.FileSystem;

namespace VRT.Downloaders.Maui
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp(builder =>
        {
            builder.Services.AddSingleton<IFileSystemService, AndroidFileSystemService>();
        });
    }
}