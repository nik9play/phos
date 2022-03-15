using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DebounceThrottle;
using fos.Tools;
using Microsoft.Toolkit.Mvvm.Input;

namespace fos.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private static uint allMonitorsBrightness;

        private static readonly ThrottleDispatcher ThrottleDispatcher =
            new((int)SettingsController.Store.AllMonitorsBrightnessChangeInterval);

        private bool _allMonitorsModeEnabled;

        public MainWindowViewModel()
        {
            Monitors = MonitorTools.GetMonitorList();
            AllMonitorsModeEnabled = SettingsController.Store.AllMonitorsModeEnabled;
            Monitors.CollectionChanged += Monitors_CollectionChanged;
        }

        public static ObservableCollection<IMonitor> Monitors { get; set; }

        public static uint AllMonitorsBrightness
        {
            get => allMonitorsBrightness;

            set
            {
                allMonitorsBrightness = value;

                ThrottleDispatcher.Throttle(() => Task.Run(() =>
                {
                    foreach (var el in Monitors) el.Brightness = value;
                }));


                RaiseStaticPropertyChanged();
            }
        }

        public bool AllMonitorsModeEnabled
        {
            get => _allMonitorsModeEnabled;

            set
            {
                _allMonitorsModeEnabled = value;
                SettingsController.Store.AllMonitorsModeEnabled = value;

                if (Monitors.Count > 0 && value)
                {
                    uint newBrightness = 0;
                    foreach (var el in Monitors)
                        newBrightness += el.Brightness;

                    newBrightness /= (uint)Monitors.Count;

                    AllMonitorsBrightness = newBrightness;
                }

                OnPropertyChanged();
            }
        }

        public RelayCommand UpdateCommand { get; } = new(() =>
        {
            Monitors.Clear();

            var list = MonitorTools.GetMonitorList();

            foreach (var el in list)
                Monitors.Add(el);
        });

        public RelayCommand OpenSettingsWindowCommand => CommonCommands.OpenSettingsWindowCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        private void Monitors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SettingsController.Store.MonitorListLocationOverwrites.Clear();
            foreach (var el in Monitors) SettingsController.Store.MonitorListLocationOverwrites.Add(el.DeviceId);

            SettingsController.SaveSettings();
        }

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
}