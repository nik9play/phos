using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace fos.Workarounds
{
    class RenderLoopFix
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
}
