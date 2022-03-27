using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace fos.Workarounds;

internal class RenderLoopFix
{
    public static void Initialize()
    {
        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
    }

    private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        ApplyFix();
    }

    private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        ApplyFix();
    }

    public static void ApplyFix()
    {
        var mainWindow = WindowManager.MainWindow;
        mainWindow?.Show();

        Task.Run(() =>
        {
            Thread.Sleep(500);
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (mainWindow != null && mainWindow.Visibility == Visibility.Visible)
                    mainWindow?.Hide();
            });
        });
    }

    ~RenderLoopFix()
    {
        SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
    }
}