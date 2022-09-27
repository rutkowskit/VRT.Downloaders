using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VRT.Downloaders.Maui.Helpers;
internal static partial class WindowHelpers
{
    [DllImport("User32.dll", SetLastError = true)]
    static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

    public static partial void BringToFront()
    {
        Process currentProcess = Process.GetCurrentProcess();
        SwitchToThisWindow(currentProcess.MainWindowHandle, true);
    }
}
