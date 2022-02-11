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

        public static void CreateWindows()
        {
            mainWindow = new MainWindow();
            hotkeyWindow = new HotkeyWindow();
        }
    }
}
