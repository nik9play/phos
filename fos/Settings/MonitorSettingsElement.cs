using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace fos
{
    class MonitorSettingsElement : INotifyPropertyChanged
    {
        private string name;
        public string Name
        { 
            get { return name; }
            
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private string deviceId;
        public string DeviceId
        { 
            get { return deviceId; }
            
            set
            {
                deviceId = value;
                OnPropertyChanged();
            }
        
        }

        public MonitorCustomLimits monitorCustomLimits;

        public int Minimum 
        {
            get { return monitorCustomLimits.Minimum; }

            set
            {
                if (value > Maximum - 10)
                {
                    if (value + 10 > 100)
                        value = 90;

                    Maximum = value + 10;
                }

                monitorCustomLimits.Minimum = value;
                SettingsController.Store.MonitorCustomLimits[DeviceId] = monitorCustomLimits;
                OnPropertyChanged();
            }
        }

        public int Maximum 
        {
            get { return monitorCustomLimits.Maximum; }

            set
            {
                if (value < Minimum + 10)
                {
                    if (value - 10 < 0)
                        value = 10;

                    Minimum = value - 10;
                }

                monitorCustomLimits.Maximum = value;
                SettingsController.Store.MonitorCustomLimits[DeviceId] = monitorCustomLimits;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
