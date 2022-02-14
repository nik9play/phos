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

        private Mutex mutex;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool aIsNewInstance;
            mutex = new Mutex(true, "phos.megaworld", out aIsNewInstance);
            if (!aIsNewInstance)
            {
                Current.Shutdown();
            }

            SettingsController.LoadSettings();
            SettingsController.LoadLanguage();
            WindowManager.CreateWindows();
            Workarounds.RenderLoopFix.Initialize();
            HotkeysManager.InitHotkeys();
        }
    }
}
