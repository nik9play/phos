using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace fos
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                SettingsController.SaveSettings();
            }
            catch { }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SettingsController.LoadSettings();
            SettingsController.LoadLanguage();
            WindowManager.CreateWindows();
            Workarounds.RenderLoopFix.Initialize();
            HotkeysManager.InitHotkeys();
        }
    }
}
