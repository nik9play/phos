using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using fos.Dialogs;
using Microsoft.Toolkit.Mvvm.Input;

namespace fos.ViewModels;

public class PageBrightnessSchedulerViewModel : INotifyPropertyChanged
{
    static PageBrightnessSchedulerViewModel()
    {
        //dialog = new SchedulerDialog();

        //OpenAddDialogCommand = new RelayCommand(() =>
        //{
        //    var element = new BrightnessSchedulerSettingsElement
        //    {
        //        AllMonitors = false,
        //        Brightness = 50,
        //        DeviceId = MainWindowViewModel.Monitors[0].DeviceId,
        //        Name = MainWindowViewModel.Monitors[0].Name,
        //        Time = new TimeSpan(12, 0, 0)
        //    };

        //    BrightnessTasks.Add(element);

        //    dialog.PrimaryButtonText = Properties.Resources.SettingsSchedulerAddButton;
        //    dialog.BrightnessTask = element;

        //    dialog.ShowAsync();

        //});

        //RemoveTaskCommand = new RelayCommand<BrightnessSchedulerSettingsElement>(task =>
        //{
        //    Debug.WriteLine(task.Brightness);
        //    BrightnessTasks.Remove(task);
        //});

        //var tasks = SettingsController.Store.BrightnessScheduler;
        //foreach (var el in tasks)
        //{
        //    var name = "Missing Monitor";

        //    var monitor = MainWindowViewModel.Monitors.FirstOrDefault(x => x.DeviceId == el.Monitor);

        //    if (monitor != null)
        //        name = monitor.Name;

        //    BrightnessTasks.Add(new BrightnessSchedulerSettingsElement
        //    {
        //        AllMonitors = el.Monitor == "all",
        //        Brightness = el.Brightness,
        //        DeviceId = el.Monitor,
        //        Time = el.Time,
        //        Name = name,
        //    });
        //}
    }

    private static SchedulerDialog dialog;

    public static RelayCommand OpenAddDialogCommand { get; }
    public static RelayCommand<BrightnessSchedulerSettingsElement> RemoveTaskCommand { get; }

    public static ObservableCollection<BrightnessSchedulerSettingsElement> BrightnessTasks { get; set; } = new();

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

    public static void RaiseStaticPropertyChanged([CallerMemberName] string prop = "")
    {
        StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(prop));
    }
}