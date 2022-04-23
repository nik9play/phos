using System;
using System.Runtime.InteropServices;
using fos.Win32Interops;

namespace fos.Monitors;

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
        if (!Dxva2.GetNumberOfPhysicalMonitorsFromHMONITOR(monitorHandle, ref _physicalMonitorsCount))
            throw new Exception("Cannot get monitor count!");
        _physicalMonitorArray = new PHYSICAL_MONITOR[_physicalMonitorsCount];

        if (!Dxva2.GetPhysicalMonitorsFromHMONITOR(monitorHandle, _physicalMonitorsCount, _physicalMonitorArray))
            throw new Exception("Cannot get physical monitor handle!");
        _firstMonitorHandle = _physicalMonitorArray[0].hPhysicalMonitor;

        if (!Dxva2.GetMonitorBrightness(_firstMonitorHandle, ref _minValue, ref _currentValue, ref _maxValue))
            throw new Exception("Cannot get monitor brightness!");
    }

    public uint Brightness => _currentValue;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void SetBrightness(uint newValue) // 0 ~ 100
    {
        newValue = Math.Min(newValue, Math.Max(0, newValue));
        _currentValue = (_maxValue - _minValue) * newValue / 100u + _minValue;
        Dxva2.SetMonitorBrightness(_firstMonitorHandle, _currentValue);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;

        if (_physicalMonitorsCount > 0)
            Dxva2.DestroyPhysicalMonitors(_physicalMonitorsCount, ref _physicalMonitorArray);
    }
}