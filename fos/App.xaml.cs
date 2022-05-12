using System;
using System.Threading;
using System.Windows;
using fos.Tools;
using fos.Win32Interops;
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
        AppMutex.CurrentMutex = new Mutex(false, "phos.megaworld");
        bool isAnotherInstanceOpen = !AppMutex.CurrentMutex.WaitOne(TimeSpan.Zero);
        if (isAnotherInstanceOpen)
        {
            Current.Shutdown();
            return;
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

            if (action == "test")
            {
                MessageBox.Show("test123");
            }
        };

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

    private void App_OnSessionEnding(object sender, SessionEndingCancelEventArgs e)
    {
        Kernel32.RegisterApplicationRestart("", Kernel32.RestartFlags.RESTART_NO_CRASH |
                                                Kernel32.RestartFlags.RESTART_NO_HANG |
                                                Kernel32.RestartFlags.RESTART_NO_REBOOT);
    }
}