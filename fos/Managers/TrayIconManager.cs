using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using fos.Properties;
using fos.Tools;
using Hardcodet.Wpf.TaskbarNotification;
using ModernWpf;
using ModernWpf.Controls;
using Size = System.Drawing.Size;

namespace fos
{
    public static class TrayIconManager
    {
        private static TaskbarIcon TrayIcon { get; set; }

        public static void InitTrayIcon()
        {
            var settingsItem = new MenuItem
            {
                Header = Resources.SettingsTitle,
                Icon = new FontIcon { Glyph = Icons.Settings },
                Command = CommonCommands.OpenSettingsWindowCommand
            };

            var exitItem = new MenuItem
            {
                Header = Resources.ContextExit,
                Icon = new FontIcon { Glyph = Icons.PowerButton },
                Command = CommonCommands.ExitApplicationCommand
            };

            var taskbarIconContextMenu = new ContextMenu
            {
                Items = { settingsItem, exitItem }
            };

            var taskbarIconToolTip = new ToolTip { Content = "phos" };

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
            var factor = VisualTreeHelper.GetDpi(WindowManager.MainWindow).DpiScaleX;
            var iconSize = new Size((int)(SystemParameters.SmallIconWidth * factor),
                (int)(SystemParameters.SmallIconHeight * factor));

            TrayIcon.Icon = new Icon(theme == ApplicationTheme.Light ? Resources.iconBlack : Resources.iconWhite,
                iconSize);
        }
    }
}