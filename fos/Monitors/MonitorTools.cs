using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using WindowsDisplayAPI.DisplayConfig;
using WindowsDisplayAPI.Native.DisplayConfig;

namespace fos;

internal static class MonitorTools
{
    private const int MONITOR_DEFAULTTONEAREST = 2;

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);


    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum,
        IntPtr dwData);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

    public static MonitorInfo GetCurrentMonitor()
    {
        GetCursorPos(out var point);
        var currentMonitorHandle = MonitorFromPoint(point, MONITOR_DEFAULTTONEAREST);
        var monitorInfoEx = new MONITORINFOEX();
        monitorInfoEx.Size = (uint)Marshal.SizeOf(monitorInfoEx);
        GetMonitorInfo(currentMonitorHandle, ref monitorInfoEx);

        return new MonitorInfo
        {
            Resolution = new Size(monitorInfoEx.Monitor.Right - monitorInfoEx.Monitor.Left,
                monitorInfoEx.Monitor.Bottom - monitorInfoEx.Monitor.Top),

            WorkingArea = new Rect(monitorInfoEx.WorkArea.Left,
                monitorInfoEx.Monitor.Top,
                monitorInfoEx.WorkArea.Right - monitorInfoEx.WorkArea.Left,
                monitorInfoEx.WorkArea.Bottom - monitorInfoEx.Monitor.Top),

            Position = new Point(monitorInfoEx.WorkArea.Left, monitorInfoEx.WorkArea.Top),
            Bounds = new Rect(monitorInfoEx.WorkArea.Left, monitorInfoEx.WorkArea.Top,
                monitorInfoEx.Monitor.Right - monitorInfoEx.Monitor.Left,
                monitorInfoEx.Monitor.Bottom - monitorInfoEx.Monitor.Top)
        };
    }

    public static ObservableCollection<IMonitor> GetMonitorList()
    {
        var MonitorHandlesDict = new Dictionary<string, IntPtr>();

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            delegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
            {
                var mi = new MONITORINFOEX();
                mi.Size = (uint)Marshal.SizeOf(mi);
                var success = GetMonitorInfo(hMonitor, ref mi);
                if (success) MonitorHandlesDict.Add(mi.DeviceName, hMonitor);
                return true;
            },
            IntPtr.Zero);

        var MonitorList = new ObservableCollection<IMonitor>();

        foreach (var pi in PathInfo.GetActivePaths())
        {
            if (!pi.IsInUse) continue;

            var name = string.IsNullOrEmpty(pi.TargetsInfo[0].DisplayTarget.FriendlyName)
                ? "Generic PnP Monitor"
                : pi.TargetsInfo[0].DisplayTarget.FriendlyName;
            var deviceId = pi.DisplaySource.DisplayName;
            if (pi.TargetsInfo[0].OutputTechnology == DisplayConfigVideoOutputTechnology.Internal)
                try
                {
                    MonitorList.Add(new InternalDisplay(pi.DisplaySource.DisplayName, pi.Resolution, pi.Position));
                }
                catch
                {
                    // ignored
                }
            else if (MonitorHandlesDict.ContainsKey(deviceId))
                try
                {
                    MonitorList.Add(new Monitor(deviceId, name, pi.Resolution, pi.Position,
                        MonitorHandlesDict[deviceId]));
                }
                catch
                {
                    // ignored
                }
        }

        var overwrites = SettingsController.Store.MonitorListLocationOverwrites;

        for (var i = 0; i < overwrites.Count; i++)
        {
            //int index = MonitorList.FindIndex(e => e.Name == overwrites[i]);
            var index = MonitorList.IndexOf(MonitorList.FirstOrDefault(el => el.DeviceId == overwrites[i]));

            if (index > -1) MonitorList.Move(index, i);
        }

        return MonitorList;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public readonly int Left;
        public readonly int Top;
        public readonly int Right;
        public readonly int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MONITORINFOEX
    {
        public uint Size;
        public readonly RECT Monitor;
        public readonly RECT WorkArea;
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

    private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor,
        IntPtr dwData);
}