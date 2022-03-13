using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using fos.Tools;
using Hardcodet.Wpf.TaskbarNotification;
using ModernWpf;
using ModernWpf.Controls;

namespace fos
{
    public static class TrayIconManager
    {
        private static TaskbarIcon TrayIcon { get; set; }

        public static void InitTrayIcon()
        {
            var settingsItem = new MenuItem
            {
                Header = Properties.Resources.SettingsTitle,
                Icon = new FontIcon { Glyph = Icons.Settings },
                Command = CommonCommands.OpenSettingsWindowCommand
            };

            var exitItem = new MenuItem
            {
                Header = Properties.Resources.ContextExit,
                Icon = new FontIcon { Glyph = Icons.PowerButton },
                Command = CommonCommands.ExitApplicationCommand
            };

            ContextMenu taskbarIconContextMenu = new ContextMenu
            {
                Items = { settingsItem, exitItem }
            };

            ToolTip taskbarIconToolTip = new ToolTip { Content = "phos" };

            TrayIcon = new TaskbarIcon
            {
                TrayToolTip = taskbarIconToolTip,
                ContextMenu = taskbarIconContextMenu,
                LeftClickCommand = CommonCommands.TogglePopupCommand,
                NoLeftClickDelay = true
            };
        }

        public static void UpdateTheme(ApplicationTheme theme)
        {
            double factor = VisualTreeHelper.GetDpi(WindowManager.MainWindow).DpiScaleX;
            var iconSize = new System.Drawing.Size((int)(SystemParameters.SmallIconWidth * factor),
                (int)(SystemParameters.SmallIconHeight * factor));
            
            TrayIcon.Icon = new System.Drawing.Icon(theme == ApplicationTheme.Light ? Properties.Resources.iconBlack : Properties.Resources.iconWhite, iconSize);
        }
    }
}
