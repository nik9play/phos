using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fos
{
    static class WindowManager
    {
        public static HotkeyWindow hotkeyWindow;
        public static MainWindow mainWindow;
        public static SettingsWindow settingsWindow = null;

        public static void CreateWindows()
        {
            mainWindow = new MainWindow();
            hotkeyWindow = new HotkeyWindow();
        }

        public static void OpenSettingsWindow()
        {
            if (settingsWindow == null)
            {
                settingsWindow = new SettingsWindow();
                settingsWindow.Closed += (s, e) => settingsWindow = null;
            }

            settingsWindow.Show();
            settingsWindow.Activate();
        }
    }
}
