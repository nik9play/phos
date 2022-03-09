using DebounceThrottle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Shapes;
using WindowsDisplayAPI.DisplayConfig;

namespace fos
{
    static class MonitorTools
    {
        private const int MONITOR_DEFAULTTONEAREST = 2;

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
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

        public class MonitorInfo
        {
            public Size Resolution { get; set; }
            public Rect WorkingArea { get; set; }
            public Point Position { get; set; }
            public Rect Bounds { get; set; }
        }

        [DllImport("user32.dll")]
        static private extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", ExactSpelling = true)]
        static private extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

        delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);


        [DllImport("user32.dll")]
        static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        public static MonitorInfo GetCurrentMonitor()
        {
            GetCursorPos(out POINT point);
            IntPtr currentMonitorHandle = MonitorFromPoint(point, MONITOR_DEFAULTTONEAREST);
            MONITORINFOEX monitorInfoEx = new MONITORINFOEX();
            monitorInfoEx.Size = (uint)Marshal.SizeOf(monitorInfoEx);
            GetMonitorInfo(currentMonitorHandle, ref monitorInfoEx);

            return new MonitorInfo
            {
                Resolution = new Size(monitorInfoEx.Monitor.Right  - monitorInfoEx.Monitor.Left,
                                      monitorInfoEx.Monitor.Bottom - monitorInfoEx.Monitor.Top),

                WorkingArea = new Rect(monitorInfoEx.WorkArea.Left,
                                       monitorInfoEx.Monitor.Top,
                                       monitorInfoEx.WorkArea.Right  - monitorInfoEx.WorkArea.Left,
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
                if (pi.TargetsInfo[0].OutputTechnology == WindowsDisplayAPI.Native.DisplayConfig.DisplayConfigVideoOutputTechnology.Internal)
                {
                    try
                    {
                        MonitorList.Add(new InternalDisplay(pi.DisplaySource.DisplayName, pi.Resolution, pi.Position));
                    }
                    catch { }
                } 
                else if (MonitorHandlesDict.ContainsKey(_deviceId))
                {
                    try
                    {
                        MonitorList.Add(new Monitor(_deviceId, _name, pi.Resolution, pi.Position, MonitorHandlesDict[_deviceId]));
                    }
                    catch { }
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
