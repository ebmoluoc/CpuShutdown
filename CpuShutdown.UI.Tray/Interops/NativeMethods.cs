using System;
using System.Runtime.InteropServices;

namespace CpuShutdown.UI.Tray.Interops
{
    internal static class NativeMethods
    {
        public const int WM_SYSCOMMAND = 0x0112;
        public const uint SC_CLOSE = 0xF060;
        public static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool PostMessage([In] IntPtr hWnd, [In] int Msg, [In] IntPtr wParam, [In] IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage([In] IntPtr hWnd, [In] int Msg, [In] IntPtr wParam, [In] IntPtr lParam);
    }
}
