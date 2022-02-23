using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace fos.ViewModels
{
    class PageMonitorsViewModel : INotifyPropertyChanged
    {
        public PageMonitorsViewModel()
        {
            foreach(var el in MainWindowViewModel.Monitors)
            {
                MonitorCustomLimits foundMonitorCustomLimits;
                SettingsController.Store.MonitorCustomLimits.TryGetValue(el.DeviceName, out foundMonitorCustomLimits);

                if (foundMonitorCustomLimits == null)
                    foundMonitorCustomLimits = new MonitorCustomLimits();

                MonitorSettings.Add(new MonitorSettingsElement
                {
                    DeviceName = el.DeviceName,
                    Name = el.Name,
                    monitorCustomLimits = foundMonitorCustomLimits
                });
            }
        }

        public ObservableCollection<MonitorSettingsElement> MonitorSettings { get; set; } = new ObservableCollection<MonitorSettingsElement>();

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
