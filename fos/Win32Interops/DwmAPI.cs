using System;
using System.Runtime.InteropServices;

namespace fos.Win32Interops;

public class DwmAPI
{
    [DllImport("dwmapi.dll")]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);

    [Flags]
    public enum DwmWindowAttribute : uint
    {
        DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
        DWMWA_WINDOW_CORNER_PREFERENCE = 33,
        DWMWA_SYSTEMBACKDROP_TYPE = 38,
        DWMWA_MICA_EFFECT = 1029,
    }

    [Flags]
    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }
}