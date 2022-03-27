using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using DebounceThrottle;
using fos.Properties;

namespace fos;

internal class InternalDisplay : IMonitor, INotifyPropertyChanged
{
    private readonly WmiBrightnessController _contoller = new();

    private readonly ThrottleDispatcher _throttleDispatcher = new(40);
    private uint _brightness;
    private string _name = Resources.InternalDisplay;

    public InternalDisplay(string deviceName, Size resolution, Point position)
    {
        DeviceName = deviceName;
        Resolution = resolution;
        Position = position;

        var newBrightness = (int)_contoller.GetBrightness();

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
    public string DeviceId => $"{DeviceName}\\InternalMonitor";

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

            _throttleDispatcher.Throttle(() => _contoller.SetBrightness(newBrightness));
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}