using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace fos.ViewModels;

internal class PageMonitorsViewModel : INotifyPropertyChanged
{
    public PageMonitorsViewModel()
    {
        foreach (var el in MainWindowViewModel.Monitors)
        {
            SettingsController.Store.MonitorCustomLimits.TryGetValue(el.DeviceId, out var foundMonitorCustomLimits);

            foundMonitorCustomLimits ??= new MonitorCustomLimits();

            MonitorSettings.Add(new MonitorSettingsElement
            {
                DeviceId = el.DeviceId,
                Name = el.Name,
                MonitorCustomLimits = foundMonitorCustomLimits
            });
        }
    }

    public ObservableCollection<MonitorSettingsElement> MonitorSettings { get; set; } = new();

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}