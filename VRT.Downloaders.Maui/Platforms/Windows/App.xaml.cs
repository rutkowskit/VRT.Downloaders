using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Devices.Geolocation;
using Windows.Graphics;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace VRT.Downloaders.Maui.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        private static SizeInt32 DefaultSize = new (900, 800);
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();            
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);            
            var appWindow = GetAppWindow(); 
            appWindow.Resize(DefaultSize);            
            appWindow.Move(GetRelativeMidPosition());
        }
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        private AppWindow GetAppWindow()
        {
            var mainWnd = Application.Windows[0].Handler!.PlatformView as MauiWinUIWindow;
            var windowId = Win32Interop.GetWindowIdFromWindow(mainWnd!.WindowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            return appWindow;
        }
        private static PointInt32 GetRelativeMidPosition()
        {
            var winSize = DeviceDisplay.MainDisplayInfo;
            var positionX = (int)winSize.Width / 2 - DefaultSize.Width / 2;
            var positionY = (int)winSize.Height / 2 - DefaultSize.Height / 2;
            return positionX < 0 || positionY < 0
                ? new PointInt32(0, 0)
                : new PointInt32(positionX, positionY);
        }
    }
}