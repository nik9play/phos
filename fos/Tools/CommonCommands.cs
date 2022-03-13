using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Toolkit.Mvvm.Input;

namespace fos.Tools
{
    internal class CommonCommands
    {
        public static RelayCommand OpenSettingsWindowCommand { get; } =
            new RelayCommand(WindowManager.OpenSettingsWindow, () => WindowManager.WindowsInitialized);

        public static RelayCommand TogglePopupCommand { get; } =
            new RelayCommand(WindowManager.MainWindow.TogglePopup, () => WindowManager.WindowsInitialized);

        public static RelayCommand ExitApplicationCommand { get; } =
            new RelayCommand(Application.Current.Shutdown);
    }
}
