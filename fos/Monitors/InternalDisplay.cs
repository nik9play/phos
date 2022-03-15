using DebounceThrottle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace fos
{
    class InternalDisplay : IMonitor, INotifyPropertyChanged
    {
        private string _name = Properties.Resources.InternalDisplay;
        private uint _brightness;
        private readonly WmiBrightnessController _contoller = new WmiBrightnessController();
        public Size Resolution { get; }
        public Point Position { get; }
        public string DeviceName { get; }
        public string DeviceId => $"{DeviceName}\\InternalMonitor";

        private readonly ThrottleDispatcher _throttleDispatcher = new ThrottleDispatcher(40);

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
                uint newBrightness = _brightness;

                SettingsController.Store.MonitorCustomLimits.TryGetValue(DeviceId, out MonitorCustomLimits monitorCustomLimits);
                if (monitorCustomLimits != null)
                    newBrightness = (uint)(((float)_brightness / 100) * (monitorCustomLimits.Maximum - (float)monitorCustomLimits.Minimum) + monitorCustomLimits.Minimum);

                _throttleDispatcher.Throttle(() => _contoller.SetBrightness(newBrightness));
                OnPropertyChanged();
            }
        }

        public InternalDisplay(string deviceName, Size resolution, Point position)
        {
            DeviceName = deviceName;
            Resolution = resolution;
            Position = position;

            int newBrightness = (int)_contoller.GetBrightness();

            SettingsController.Store.MonitorCustomLimits.TryGetValue(DeviceId, out MonitorCustomLimits monitorCustomLimits);
            if (monitorCustomLimits != null)
                newBrightness = (int)((newBrightness - (float)monitorCustomLimits.Minimum) / (monitorCustomLimits.Maximum - (float)monitorCustomLimits.Minimum) * 100);

            if (newBrightness < 0)
                newBrightness = 0;
            if (newBrightness > 100)
                newBrightness = 100;

            _brightness = (uint)newBrightness;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
