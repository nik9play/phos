using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DebounceThrottle;

namespace fos
{
    public class Monitor : IMonitor, INotifyPropertyChanged
    {
        private string _name;
        private uint _brightness;
        private readonly BrightnessController _contoller;

        private readonly ThrottleDispatcher _throttleDispatcher = new ThrottleDispatcher((int)SettingsController.Store.BrightnessChangeInterval);
        public Size Resolution { get; }
        public Point Position { get; }
        public string DeviceName { get; }

        public string DeviceId { get => $"{DeviceName}\\{Name}"; }

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

                _throttleDispatcher.Throttle(() => Task.Run(() => _contoller.SetBrightness(newBrightness)));
                OnPropertyChanged();
            }
        }

        public void SetBrightness(uint brightness)
        {
            Brightness = brightness;
        }

        public Monitor(string deviceName, string name, Size resolution, Point position, IntPtr monitorHandle)
        {
            DeviceName = deviceName;
            _name = name;
            Resolution = resolution;
            Position = position;

            _contoller = new BrightnessController(monitorHandle);

            int newBrightness = (int)_contoller.Brightness;

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
