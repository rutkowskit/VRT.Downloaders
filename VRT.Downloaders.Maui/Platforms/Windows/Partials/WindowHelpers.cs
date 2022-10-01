using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VRT.Downloaders.Maui.Helpers;
internal static partial class WindowHelpers
{
    [DllImport("User32.dll", SetLastError = true)]
    static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    static readonly IntPtr HWND_TOPMOST = new (-1);
    static readonly IntPtr HWND_NOTOPMOST = new(-2);
    const uint SWP_NOSIZE = 0x0001;
    const uint SWP_NOMOVE = 0x0002;
    const uint SWP_SHOWWINDOW = 0x0040;

    public static partial void BringToFront()
    {
        Process currentProcess = Process.GetCurrentProcess();
        var hwnd = currentProcess.MainWindowHandle;
        SwitchToThisWindow(hwnd, true);
        hwnd.SetTopMost(true);
        hwnd.SetTopMost(false);
    }
    private static void SetTopMost(this IntPtr hwnd, bool topmost)
    {
        var insertAfter = topmost ? HWND_TOPMOST : HWND_NOTOPMOST;
        SetWindowPos(hwnd, insertAfter, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
    }
}
