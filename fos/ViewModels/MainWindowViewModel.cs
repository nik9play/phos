using DebounceThrottle;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace fos.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private static uint _allMonitorsBrightness;
        private static readonly ThrottleDispatcher _throttleDispatcher = new ThrottleDispatcher((int)SettingsController.Store.AllMonitorsBrightnessChangeInterval);
        public static ObservableCollection<IMonitor> Monitors { get; set; }

        public static uint AllMonitorsBrightness { 
            get
            {
                return _allMonitorsBrightness;
            }

            set
            {
                _allMonitorsBrightness = value;

                _throttleDispatcher.Throttle(() => Task.Run(() =>
                {
                    foreach (var el in Monitors)
                    {
                        el.Brightness = value;
                    }
                }));


                RaiseStaticPropertyChanged();
            }
        }

        private bool _allMonitorsModeEnabled;
        public bool AllMonitorsModeEnabled
        {
            get
            {
                return _allMonitorsModeEnabled;
            }

            set
            {
                _allMonitorsModeEnabled = value;
                SettingsController.Store.AllMonitorsModeEnabled = value;

                if (Monitors.Count > 0 && value)
                {
                    uint newBrightness = 0;
                    foreach (IMonitor el in Monitors)
                        newBrightness += el.Brightness;

                    newBrightness /= (uint)Monitors.Count;

                    AllMonitorsBrightness = newBrightness;
                }

                OnPropertyChanged();
            }
        }

        //private RelayCommand updateCommand;
        //public RelayCommand UpdateCommand
        //{
        //    get
        //    {
        //        return updateCommand ??
        //          (updateCommand = new RelayCommand(obj =>
        //          {
        //              Monitors.Clear();

        //              var list = MonitorTools.GetMonitorList();

        //              foreach(var el in list)
        //                  Monitors.Add(el);
        //          }));
        //    }
        //}

        public ICommand UpdateCommand { get; }

        private void Update()
        {
            Monitors.Clear();

            var list = MonitorTools.GetMonitorList();

            foreach (var el in list)
                Monitors.Add(el);
        }

        public MainWindowViewModel()
        {
            Monitors = MonitorTools.GetMonitorList();
            AllMonitorsModeEnabled = SettingsController.Store.AllMonitorsModeEnabled;
            Monitors.CollectionChanged += Monitors_CollectionChanged;

            UpdateCommand = new RelayCommand(() => Update());
        }

        private void Monitors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine(e.Action.ToString());
            SettingsController.Store.MonitorListLocationOverwrites.Clear();
            foreach (IMonitor el in Monitors)
            {
                SettingsController.Store.MonitorListLocationOverwrites.Add(el.DeviceId);
            }
        }

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
}
