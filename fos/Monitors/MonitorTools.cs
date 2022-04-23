using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using fos.Win32Interops;
using WindowsDisplayAPI.DisplayConfig;
using WindowsDisplayAPI.Native.DisplayConfig;

namespace fos.Monitors;

internal static class MonitorTools
{
    public static User32.MonitorInfo GetCurrentMonitor()
    {
        User32.GetCursorPos(out var point);
        var currentMonitorHandle = User32.MonitorFromPoint(point, User32.MONITOR_DEFAULTTONEAREST);
        var monitorInfoEx = new User32.MONITORINFOEX();
        monitorInfoEx.Size = (uint)Marshal.SizeOf(monitorInfoEx);
        User32.GetMonitorInfo(currentMonitorHandle, ref monitorInfoEx);

        return new User32.MonitorInfo
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

        User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
            delegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Shell32.RECT lprcMonitor, IntPtr dwData)
            {
                var mi = new User32.MONITORINFOEX();
                mi.Size = (uint)Marshal.SizeOf(mi);
                var success = User32.GetMonitorInfo(hMonitor, ref mi);
                if (success) MonitorHandlesDict.Add(mi.DeviceName, hMonitor);
                return true;
            },
            IntPtr.Zero);

        var monitorList = new ObservableCollection<IMonitor>();

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
                    monitorList.Add(new InternalDisplay(pi.DisplaySource.DisplayName, pi.Resolution, pi.Position));
                }
                catch
                {
                    // ignored
                }
            else if (MonitorHandlesDict.ContainsKey(deviceId))
                try
                {
                    monitorList.Add(new Monitor(deviceId, name, pi.Resolution, pi.Position,
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
            var index = monitorList.IndexOf(monitorList.FirstOrDefault(el => el.DeviceId == overwrites[i]));

            if (index > -1) monitorList.Move(index, i);
        }

        return monitorList;
    }
}