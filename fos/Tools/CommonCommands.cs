using System.Windows;
using Microsoft.Toolkit.Mvvm.Input;

namespace fos.Tools
{
    internal class CommonCommands
    {
        public static RelayCommand OpenSettingsWindowCommand { get; } =
            new(WindowManager.OpenSettingsWindow, () => WindowManager.WindowsInitialized);

        public static RelayCommand TogglePopupCommand { get; } =
            new(WindowManager.MainWindow.TogglePopup, () => WindowManager.WindowsInitialized);

        public static RelayCommand ExitApplicationCommand { get; } = new(Application.Current.Shutdown);
    }
}