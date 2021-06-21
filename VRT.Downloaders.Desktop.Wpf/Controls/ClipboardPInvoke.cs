using System;
using System.Runtime.InteropServices;

namespace VRT.Downloaders.Desktop.Wpf.Controls
{
    public static class ClipboardPInvoke
    {
        [DllImport("User32.dll")]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove,
            IntPtr hWndNewNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg,
                                             IntPtr wParam,
                                             IntPtr lParam);
    }
}
