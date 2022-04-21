using System;
using System.Runtime.InteropServices;

namespace fos;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct PHYSICAL_MONITOR
{
    public IntPtr hPhysicalMonitor;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string szPhysicalMonitorDescription;
}

internal class BrightnessController : IDisposable
{
    private readonly IntPtr _firstMonitorHandle;
    private readonly uint _maxValue;

    private readonly uint _minValue;

    private readonly uint _physicalMonitorsCount;
    private uint _currentValue;
    private PHYSICAL_MONITOR[] _physicalMonitorArray;

    public BrightnessController(IntPtr monitorHandle)
    {
        if (!GetNumberOfPhysicalMonitorsFromHMONITOR(monitorHandle, ref _physicalMonitorsCount))
            throw new Exception("Cannot get monitor count!");
        _physicalMonitorArray = new PHYSICAL_MONITOR[_physicalMonitorsCount];

        if (!GetPhysicalMonitorsFromHMONITOR(monitorHandle, _physicalMonitorsCount, _physicalMonitorArray))
            throw new Exception("Cannot get physical monitor handle!");
        _firstMonitorHandle = _physicalMonitorArray[0].hPhysicalMonitor;

        if (!GetMonitorBrightness(_firstMonitorHandle, ref _minValue, ref _currentValue, ref _maxValue))
            throw new Exception("Cannot get monitor brightness!");
    }

    public uint Brightness => _currentValue;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize,
        ref PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor,
        ref uint pdwNumberOfPhysicalMonitors);

    [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize,
        [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

    [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness,
        ref uint currentBrightness, ref uint maxBrightness);

    [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);

    public void SetBrightness(uint newValue) // 0 ~ 100
    {
        newValue = Math.Min(newValue, Math.Max(0, newValue));
        _currentValue = (_maxValue - _minValue) * newValue / 100u + _minValue;
        SetMonitorBrightness(_firstMonitorHandle, _currentValue);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;

        if (_physicalMonitorsCount > 0)
            DestroyPhysicalMonitors(_physicalMonitorsCount, ref _physicalMonitorArray);
    }
}