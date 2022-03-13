using Microsoft.Toolkit.Uwp.Notifications;
using ModernWpf;
using System.Threading;
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

            ToastNotificationManagerCompat.Uninstall();
        }

        private Mutex _mutex;
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            _mutex = new Mutex(true, "phos.megaworld", out var aIsNewInstance);
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
            TrayIconManager.InitTrayIcon();

            UpdateTheme(ThemeTools.CurrentTheme);
            ThemeTools.ThemeChanged += ThemeTools_ThemeChanged;

            if (SettingsController.Store.AutoUpdateCheckEnabled)
            {
                UpdateManager.StartTimer();
                await UpdateManager.CheckUpdatesSilent();
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
                        WindowManager.SettingsWindow.OpenAboutPage();
                    });
                }
            };
        }

        private void ThemeTools_ThemeChanged(object sender, ThemeChangingArgs e)
        {
            UpdateTheme(e.CurrentTheme);
        }

        private void UpdateTheme(ApplicationTheme currentTheme)
        {
            ThemeManager.Current.ApplicationTheme = currentTheme;
            TrayIconManager.UpdateTheme(currentTheme);
        }
    }
}
