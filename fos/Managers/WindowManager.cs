namespace fos;

internal static class WindowManager
{
    public static HotkeyWindow HotkeyWindow { get; private set; }
    public static MainWindow MainWindow { get; private set; }
    public static SettingsWindow SettingsWindow { get; private set; }
    public static WelcomeWindow WelcomeWindow { get; private set; }
    public static bool WindowsInitialized { get; private set; }

    public static void CreateWindows()
    {
        MainWindow = new MainWindow();
        MainWindow.PreloadWindow();

        HotkeyWindow = new HotkeyWindow();

        if (SettingsController.Store.FirstStart)
        {
            WelcomeWindow = new WelcomeWindow();
            WelcomeWindow.Show();
        }

        WindowsInitialized = true;
    }

    public static void OpenSettingsWindow()
    {
        SettingsController.LoadLanguage();

        if (SettingsWindow == null)
        {
            SettingsWindow = new SettingsWindow();
            SettingsWindow.Closed += (s, e) => SettingsWindow = null;
        }

        SettingsWindow.Show();
        SettingsWindow.Activate();
    }
}