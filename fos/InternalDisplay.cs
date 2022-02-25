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
        private WmiBrightnessController _contoller = new WmiBrightnessController();
        public Size Resolution { get; set; }
        public Point Position { get; set; }
        private string _deviceName;
        public string DeviceName { get { return _deviceName; } }
        public string DeviceId { get => $"{DeviceName}\\InternalMonitor"; }

        private readonly ThrottleDispatcher _throttleDispatcher = new ThrottleDispatcher(40);

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public uint Brightness
        {
            get { return _brightness; }
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

        public void SetBrightness(uint brightness)
        {
            Brightness = brightness;
        }

        public InternalDisplay(string deviceName)
        {
            _deviceName = deviceName;

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
