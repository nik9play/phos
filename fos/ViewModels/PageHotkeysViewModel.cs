using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace fos.ViewModels
{
    class PageHotkeysViewModel : INotifyPropertyChanged
    {
        private bool hotkeysEnabled = SettingsController.Store.HotkeysEnabled;
        public bool HotkeysEnabled
        {
            get
            {
                return hotkeysEnabled;
            }

            set
            {
                hotkeysEnabled = value;
                SettingsController.Store.HotkeysEnabled = value;

                if (value) HotkeysManager.InitHotkeys();
                else HotkeysManager.RemoveHotkeys();

                OnPropertyChanged();
            }
        }

        private string hotkeyUp = SettingsController.Store.HotkeyUp;
        public string HotkeyUp
        {
            get
            {
                return hotkeyUp;
            }

            set
            {
                hotkeyUp = value;
                SettingsController.Store.HotkeyUp = value;

                if (string.IsNullOrEmpty(value))
                {
                    hotkeyUp = SettingsController.defaultSettings.HotkeyUp;
                    SettingsController.Store.HotkeyUp = SettingsController.defaultSettings.HotkeyUp;
                }

                HotkeysManager.RemoveHotkeys();
                HotkeysManager.InitHotkeys();

                OnPropertyChanged();
            }
        }

        private string hotkeyDown = SettingsController.Store.HotkeyDown;
        public string HotkeyDown
        {
            get
            {
                return hotkeyDown;
            }

            set
            {
                hotkeyDown = value;
                SettingsController.Store.HotkeyDown = value;

                if (string.IsNullOrEmpty(value))
                {
                    hotkeyDown = SettingsController.defaultSettings.HotkeyDown;
                    SettingsController.Store.HotkeyDown = SettingsController.defaultSettings.HotkeyDown;
                }

                HotkeysManager.RemoveHotkeys();
                HotkeysManager.InitHotkeys();

                OnPropertyChanged();
            }
        }

        private uint hotkeyStep = SettingsController.Store.HotkeyStep;
        public uint HotkeyStep
        {
            get
            {
                return hotkeyStep;
            }

            set
            {
                hotkeyStep = value;
                SettingsController.Store.HotkeyStep = value;

                OnPropertyChanged();
            }
        }

        private HotkeyPopupLocationEnum hotkeyPopupLocation = SettingsController.Store.HotkeyPopupLocation;
        public HotkeyPopupLocationEnum HotkeyPopupLocation
        {
            get
            {
                return hotkeyPopupLocation;
            }
            set { 
                hotkeyPopupLocation = value;
                SettingsController.Store.HotkeyPopupLocation = value;
                
                OnPropertyChanged();
            }
        }

        public PageHotkeysViewModel()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
