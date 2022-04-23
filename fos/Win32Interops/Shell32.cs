using System;
using System.Runtime.InteropServices;

namespace fos.Win32Interops;

public static class Shell32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NOTIFYICONIDENTIFIER
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uID;
        public Guid guidItem;
    }

    [DllImport("shell32.dll", SetLastError = true)]
    private static extern long Shell_NotifyIconGetRect([In] ref NOTIFYICONIDENTIFIER identifier, [Out] out RECT iconLocation);

    public static RECT GetNotifyIconRect(Guid taskbarIconGuid)
    {
        var notifyIcon = new NOTIFYICONIDENTIFIER();
        notifyIcon.cbSize = (uint)Marshal.SizeOf(notifyIcon.GetType());
        notifyIcon.guidItem = taskbarIconGuid;

        var hresult = Shell_NotifyIconGetRect(ref notifyIcon, out var rect);
        if (hresult == 0x80004005)
        {
            throw new Exception("Failed to get icon position.");
        }

        if (hresult != 0)
        {
            throw new Exception("Unknown error");
        }

        return rect;
    }
}