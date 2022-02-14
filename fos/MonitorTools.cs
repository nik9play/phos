using DebounceThrottle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using WindowsDisplayAPI.DisplayConfig;

namespace fos
{
    static class MonitorTools
    {
        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct MONITORINFOEX
        {
            public uint Size;
            public RECT Monitor;
            public RECT WorkArea;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
        }

        delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        public static List<IMonitor> GetMonitorList()
        {   
            var MonitorDict = new Dictionary<string, Monitor>();

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
                {
                    MONITORINFOEX mi = new MONITORINFOEX();
                    mi.Size = (uint)Marshal.SizeOf(mi);
                    bool success = GetMonitorInfo(hMonitor, ref mi);
                    if (success)
                    {
                        try
                        {
                            var mon = new Monitor(mi.DeviceName, hMonitor);
                            MonitorDict.Add(mon.deviceName, mon);
                        } catch { }
                    }
                    return true;
                },
            IntPtr.Zero);

            List<IMonitor> MonitorList = new List<IMonitor>();

            foreach (PathInfo pi in PathInfo.GetActivePaths())
            {
                if (!pi.IsInUse) continue;

                string _name = string.IsNullOrEmpty(pi.TargetsInfo[0].DisplayTarget.FriendlyName) ? "Generic PnP Monitor" : pi.TargetsInfo[0].DisplayTarget.FriendlyName;
                string _deviceId = pi.DisplaySource.DisplayName;

                if (MonitorDict.ContainsKey(_deviceId))
                {
                    MonitorDict[_deviceId].Name = _name;
                    MonitorDict[_deviceId].Resolution = pi.Resolution;
                    MonitorDict[_deviceId].Position = pi.Position;

                    MonitorList.Add(MonitorDict[_deviceId]);
                }
                else if (pi.TargetsInfo[0].OutputTechnology == WindowsDisplayAPI.Native.DisplayConfig.DisplayConfigVideoOutputTechnology.Internal)
                {
                    try
                    {
                        InternalDisplay display = new InternalDisplay(pi.DisplaySource.DisplayName);
                        display.Resolution = pi.Resolution;
                        display.Position = pi.Position;

                        MonitorList.Add(display);
                    }
                    catch { }
                }
            }

            return MonitorList;
        }
    }
}
