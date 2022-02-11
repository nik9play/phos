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
        public string DeviceName { get; }

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
                _throttleDispatcher.Throttle(() => _contoller.SetBrightness(_brightness));
                OnPropertyChanged();
            }
        }

        public void SetBrightness(uint brightness)
        {
            Brightness = brightness;
        }

        public InternalDisplay()
        {
            _brightness = _contoller.GetBrightness();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
