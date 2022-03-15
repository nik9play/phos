namespace fos
{
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
            HotkeyWindow = new HotkeyWindow();
            SettingsWindow = new SettingsWindow();

            if (SettingsController.Store.FirstStart)
            {
                WelcomeWindow = new WelcomeWindow();
                WelcomeWindow.Show();
            }

            WindowsInitialized = true;
        }

        public static void OpenSettingsWindow()
        {
            SettingsWindow.Show();
            SettingsWindow.Activate();
        }
    }
}