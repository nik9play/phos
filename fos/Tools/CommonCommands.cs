using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Toolkit.Mvvm.Input;

namespace fos.Tools
{
    public static class CommonCommands
    {
        public static RelayCommand OpenSettingsWindowCommand { get; } =
            new(WindowManager.OpenSettingsWindow, () => WindowManager.WindowsInitialized);

        public static RelayCommand RestartApplicationCommand { get; } = new(() =>
        {
            AppMutex.CurrentMutex.ReleaseMutex();
            AppMutex.CurrentMutex.Dispose();
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "phos.exe");
            process.Start();
            Application.Current.Shutdown();
        });

        public static RelayCommand TogglePopupCommand { get; } =
            new(WindowManager.MainWindow.TogglePopup, () => WindowManager.WindowsInitialized);

        public static RelayCommand ExitApplicationCommand { get; } = new(Application.Current.Shutdown);
    }
}