using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace fos
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PHYSICAL_MONITOR
    {
        public IntPtr hPhysicalMonitor;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szPhysicalMonitorDescription;
    }

    class BrightnessController : IDisposable
    {
        [DllImport("user32.dll", EntryPoint = "MonitorFromWindow")]
        public static extern IntPtr MonitorFromWindow([In] IntPtr hwnd, uint dwFlags);

        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, ref PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness, ref uint currentBrightness, ref uint maxBrightness);

        [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);

        private uint _physicalMonitorsCount = 0;
        private PHYSICAL_MONITOR[] _physicalMonitorArray;

        private IntPtr _firstMonitorHandle;

        private uint _minValue = 0;
        private uint _maxValue = 0;
        private uint _currentValue = 0;

        public uint Brightness
        {
            get
            {
                return _currentValue;
            }
        }

        public BrightnessController(IntPtr monitorHandle)
        {
            if (!GetNumberOfPhysicalMonitorsFromHMONITOR(monitorHandle, ref _physicalMonitorsCount))
            {
                throw new Exception("Cannot get monitor count!");
            }
            _physicalMonitorArray = new PHYSICAL_MONITOR[_physicalMonitorsCount];

            if (!GetPhysicalMonitorsFromHMONITOR(monitorHandle, _physicalMonitorsCount, _physicalMonitorArray))
            {
                throw new Exception("Cannot get physical monitor handle!");
            }
            _firstMonitorHandle = _physicalMonitorArray[0].hPhysicalMonitor;

            if (!GetMonitorBrightness(_firstMonitorHandle, ref _minValue, ref _currentValue, ref _maxValue))
            {
                throw new Exception("Cannot get monitor brightness!");
            }
        }

        public void SetBrightness(uint newValue) // 0 ~ 100
        {
            newValue = Math.Min(newValue, Math.Max(0, newValue));
            _currentValue = ((_maxValue - _minValue) * newValue / 100u) + _minValue;
            SetMonitorBrightness(_firstMonitorHandle, _currentValue);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_physicalMonitorsCount > 0)
                {
                    DestroyPhysicalMonitors(_physicalMonitorsCount, ref _physicalMonitorArray);
                }
            }
        }
    }
}
