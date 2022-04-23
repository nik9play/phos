using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DebounceThrottle;
using fos.ViewModels;

namespace fos.Monitors;

public class Monitor : IMonitor, INotifyPropertyChanged
{
    private readonly BrightnessController _contoller;

    private readonly ThrottleDispatcher _throttleDispatcher =
        new((int)SettingsController.Store.BrightnessChangeInterval);

    private readonly ThrottleDispatcher _slowThrottleDispatcher =
        new((int)SettingsController.Store.TrayIconBrightnessChangeInterval);

    private uint _brightness;
    private string _name;

    public Monitor(string deviceName, string name, Size resolution, Point position, IntPtr monitorHandle)
    {
        DeviceName = deviceName;
        Name = name;
        Resolution = resolution;
        Position = position;

        _contoller = new BrightnessController(monitorHandle);

        var newBrightness = (int)_contoller.Brightness;

        SettingsController.Store.MonitorCustomLimits.TryGetValue(DeviceId, out var monitorCustomLimits);
        if (monitorCustomLimits != null)
            newBrightness = (int)((newBrightness - (float)monitorCustomLimits.Minimum) /
                (monitorCustomLimits.Maximum - (float)monitorCustomLimits.Minimum) * 100);

        if (newBrightness < 0)
            newBrightness = 0;
        if (newBrightness > 100)
            newBrightness = 100;

        _brightness = (uint)newBrightness;
    }

    public Size Resolution { get; }
    public Point Position { get; }
    public string DeviceName { get; }

    public string DeviceId => $"{DeviceName}\\{Name}";

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public uint Brightness
    {
        get => _brightness;
        set
        {
            _brightness = value;
            var newBrightness = _brightness;

            SettingsController.Store.MonitorCustomLimits.TryGetValue(DeviceId, out var monitorCustomLimits);
            if (monitorCustomLimits != null)
                newBrightness =
                    (uint)((float)_brightness / 100 *
                           (monitorCustomLimits.Maximum - (float)monitorCustomLimits.Minimum) +
                           monitorCustomLimits.Minimum);

            _throttleDispatcher.Throttle(() => Task.Run(() => _contoller.SetBrightness(newBrightness)));
            OnPropertyChanged();
        }
    }

    public void SetBrightnessSlow(uint brightness)
    {
        _brightness = brightness;
        var newBrightness = _brightness;

        SettingsController.Store.MonitorCustomLimits.TryGetValue(DeviceId, out var monitorCustomLimits);
        if (monitorCustomLimits != null)
            newBrightness =
                (uint)((float)_brightness / 100 *
                       (monitorCustomLimits.Maximum - (float)monitorCustomLimits.Minimum) +
                       monitorCustomLimits.Minimum);

        _slowThrottleDispatcher.Throttle(() => Task.Run(() => _contoller.SetBrightness(newBrightness)));
        OnPropertyChanged(nameof(Brightness));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}