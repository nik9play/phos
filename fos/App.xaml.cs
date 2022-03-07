using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Foundation.Collections;

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

            ToastNotificationManagerCompat.Uninstall();
        }

        private Mutex mutex;
        private async void Application_Startup(object sender, StartupEventArgs e)
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
            UpdateManager.InitTimer();

            if (SettingsController.Store.AutoUpdateCheckEnabled)
            {
                UpdateManager.StartTimer();
            }

            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                args.TryGetValue("action", out string action);
                
                if (action == "update")
                {
                    Current.Dispatcher.Invoke(delegate
                    {
                        WindowManager.OpenSettingsWindow();
                        WindowManager.settingsWindow.OpenAboutPage();
                    });
                } 
                else if (action == "restartApp")
                {
                    Current.Dispatcher.Invoke(delegate
                    {
                        //Debug.WriteLine(System.Reflection.Assembly.GetEntryAssembly().Location);
                        //Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    });
                }
            };

            await UpdateManager.CheckUpdatesSilent();
        }
    }
}
