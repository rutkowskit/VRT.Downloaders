using System.Windows;

namespace VRT.Downloaders.Desktop.Wpf
{
    public static class WindowExtensions
    {
        public static void BringToFront(this Window window)
        {
            Guard.AgainstNull(window, nameof(window)).Discard();
            if (!window.IsVisible)
                window.Show();

            if (window.WindowState == WindowState.Minimized)
                window.WindowState = WindowState.Normal;
            window.Activate().Discard();
            window.Topmost = true;
            window.Topmost = false;
            window.Focus().Discard();
        }
    }
}
