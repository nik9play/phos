using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace fos.ViewModels;

public class BrightnessSchedulerSettingsElement : INotifyPropertyChanged
{
    private bool _allMonitors;
    private string _name;
    private string _deviceId;
    private TimeSpan _time;
    private uint _brightness;

    public bool AllMonitors
    {
        get => _allMonitors;
        set
        {
            _allMonitors = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public string DeviceId
    {
        get => _deviceId;
        set
        {
            _deviceId = value;
            OnPropertyChanged();
        }
    }

    public TimeSpan Time
    {
        get => _time;
        set
        {
            _time = value;
            OnPropertyChanged();
        }
    }

    public uint Brightness
    {
        get => _brightness;
        set
        {
            _brightness = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}