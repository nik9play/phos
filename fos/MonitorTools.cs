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

        public static ObservableCollection<IMonitor> GetMonitorList()
        {   
            var MonitorHandlesDict = new Dictionary<string, IntPtr>();

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
                {
                    MONITORINFOEX mi = new MONITORINFOEX();
                    mi.Size = (uint)Marshal.SizeOf(mi);
                    bool success = GetMonitorInfo(hMonitor, ref mi);
                    if (success)
                    {
                        MonitorHandlesDict.Add(mi.DeviceName, hMonitor);
                    }
                    return true;
                },
            IntPtr.Zero);

            ObservableCollection<IMonitor> MonitorList = new ObservableCollection<IMonitor>();

            foreach (PathInfo pi in PathInfo.GetActivePaths())
            {
                if (!pi.IsInUse) continue;

                string _name = string.IsNullOrEmpty(pi.TargetsInfo[0].DisplayTarget.FriendlyName) ? "Generic PnP Monitor" : pi.TargetsInfo[0].DisplayTarget.FriendlyName;
                string _deviceId = pi.DisplaySource.DisplayName;

                if (MonitorHandlesDict.ContainsKey(_deviceId))
                {
                    try
                    {
                        MonitorList.Add(new Monitor(_deviceId, _name, pi.Resolution, pi.Position, MonitorHandlesDict[_deviceId]));
                    } catch { }
                }
                else if (pi.TargetsInfo[0].OutputTechnology == WindowsDisplayAPI.Native.DisplayConfig.DisplayConfigVideoOutputTechnology.Internal)
                {
                    try
                    {
                        MonitorList.Add(new InternalDisplay(pi.DisplaySource.DisplayName, pi.Resolution, pi.Position));
                    } catch { }
                }
            }

            List<string> overwrites = SettingsController.Store.MonitorListLocationOverwrites;

            for (int i = 0; i < overwrites.Count; i++)
            {
                //int index = MonitorList.FindIndex(e => e.Name == overwrites[i]);
                int index = MonitorList.IndexOf(MonitorList.Where(el => el.DeviceId == overwrites[i]).FirstOrDefault());

                if (index > -1)
                {
                    MonitorList.Move(index, i);
                }
            }

            return MonitorList;
        }
    }
}
