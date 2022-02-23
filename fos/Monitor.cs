using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public readonly string deviceName;
        private readonly BrightnessController _contoller;

        private readonly ThrottleDispatcher _throttleDispatcher = new ThrottleDispatcher((int)SettingsController.Store.BrightnessChangeInterval);
        public Size Resolution { get; set; }
        public Point Position { get; set; }
        public string DeviceName
        {
            get { return deviceName; }
        }
        
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
                _throttleDispatcher.Throttle(() => Task.Run(() => _contoller.SetBrightness(_brightness)));
                OnPropertyChanged();
            }
        }

        public void SetBrightness(uint brightness)
        {
            Brightness = brightness;
            _contoller.SetBrightness(brightness);
        }

        public Monitor(string deviceName, IntPtr monitorHandle)
        {
            this.deviceName = deviceName;

            _contoller = new BrightnessController(monitorHandle);
            Brightness = _contoller.Brightness;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
