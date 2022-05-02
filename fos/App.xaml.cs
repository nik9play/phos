using System.Diagnostics;
using System.Threading;
using System.Windows;
using fos.Tools;
using fos.Workarounds;
using Microsoft.Toolkit.Uwp.Notifications;
using ModernWpf;

namespace fos;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Exit(object sender, ExitEventArgs e)
    {
        try
        {
            SettingsController.SaveSettings();
        }
        catch
        {
            // ignored
        }

        ToastNotificationManagerCompat.Uninstall();
    }

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        AppMutex.CurrentMutex = new Mutex(false, "phos.megaworld", out var aIsNewInstance);
        if (!aIsNewInstance)
        {
            Current.Shutdown();
            return;
        }

        SettingsController.LoadSettings();
        SettingsController.LoadLanguage();
        WindowManager.CreateWindows();
        RenderLoopFix.Initialize();
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
            var args = ToastArguments.Parse(toastArgs.Argument);

            args.TryGetValue("action", out var action);

            if (action == "update")
                Current.Dispatcher.Invoke(delegate
                {
                    WindowManager.OpenSettingsWindow();
                    WindowManager.SettingsWindow.OpenAboutPage();
                });
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