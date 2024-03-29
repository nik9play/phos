﻿using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace fos.Win32Interops;

public static class User32
{
    public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Shell32.RECT lprcMonitor,
        IntPtr dwData);

    public const int MONITOR_DEFAULTTONEAREST = 2;

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);


    [DllImport("user32.dll")]
    public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum,
        IntPtr dwData);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int MoveWindow(IntPtr hWnd, int X, int Y,
        int nWidth, int nHeight,
        [MarshalAs(UnmanagedType.Bool)] bool bRepaint);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MONITORINFOEX
    {
        public uint Size;
        public readonly Shell32.RECT Monitor;
        public readonly Shell32.RECT WorkArea;
        public readonly uint Flags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public readonly string DeviceName;
    }

    public class MonitorInfo
    {
        public Size Resolution { get; set; }
        public Rect WorkingArea { get; set; }
        public Point Position { get; set; }
        public Rect Bounds { get; set; }
    }
}