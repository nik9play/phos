using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace fos;

internal class MonitorSettingsElement : INotifyPropertyChanged
{
    private string _deviceId;
    private string _name;

    public MonitorCustomLimits MonitorCustomLimits;

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

    public int Minimum
    {
        get => MonitorCustomLimits.Minimum;

        set
        {
            if (value > Maximum - 10)
            {
                if (value + 10 > 100)
                    value = 90;

                Maximum = value + 10;
            }

            MonitorCustomLimits.Minimum = value;
            SettingsController.Store.MonitorCustomLimits[DeviceId] = MonitorCustomLimits;
            OnPropertyChanged();
        }
    }

    public int Maximum
    {
        get => MonitorCustomLimits.Maximum;

        set
        {
            if (value < Minimum + 10)
            {
                if (value - 10 < 0)
                    value = 10;

                Minimum = value - 10;
            }

            MonitorCustomLimits.Maximum = value;
            SettingsController.Store.MonitorCustomLimits[DeviceId] = MonitorCustomLimits;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}