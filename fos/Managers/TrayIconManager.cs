using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using DebounceThrottle;
using fos.Properties;
using fos.Tools;
using fos.ViewModels;
using fos.Win32Interops;
using H.Hooks;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using ModernWpf;
using ModernWpf.Controls;
using Size = System.Drawing.Size;

namespace fos;

public static class TrayIconManager
{
    private static readonly ThrottleDispatcher ThrottleDispatcher =
        new((int)SettingsController.Store.AllMonitorsBrightnessChangeInterval);

    private static TaskbarIcon IconInTray { get; set; }
    private static Guid TrayIconGuid { get; set; }
    private static LowLevelMouseHook MouseHook { get; set; }

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

        IconInTray = new TaskbarIcon
        {
            TrayToolTip = taskbarIconToolTip,
            ToolTipText = "phos",
            ContextMenu = taskbarIconContextMenu,
            LeftClickCommand = CommonCommands.TogglePopupCommand,
            NoLeftClickDelay = true
        };

        TrayIconGuid = TrayIcon.CreateUniqueGuidForEntryAssembly();

        MouseHook = new LowLevelMouseHook();

        IconInTray.ForceCreate();

        MouseHook.Wheel += OnMouseWheel;

        MouseHook.Start();
    }

    private static void OnMouseWheel(object sender, MouseEventArgs args)
    {

            var trayIconRectangle = GetRectangle();

            if (trayIconRectangle.HasValue &&
                args.Position.X > trayIconRectangle?.X &&
                args.Position.X < trayIconRectangle.Value.X + trayIconRectangle.Value.Width &&
                args.Position.Y > trayIconRectangle.Value.Y &&
                args.Position.Y < trayIconRectangle.Value.Y + trayIconRectangle.Value.Height)
                Application.Current.Dispatcher.Invoke(delegate
                {
                    var offset = args.Delta > 0 ? 5 : -5;

                    try
                    {
                        if (SettingsController.Store.AllMonitorsModeEnabled)
                        {
                            var newBrightness = MainWindowViewModel.AllMonitorsBrightness + offset;

                            if (newBrightness < 0)
                                newBrightness = 0;

                            if (newBrightness > 100)
                                newBrightness = 100;

                            MainWindowViewModel.AllMonitorsBrightness = (uint)newBrightness;
                        }
                        else
                        {

                            foreach (var el in MainWindowViewModel.Monitors)
                            {
                                var newBrightness = el.Brightness + offset;

                                if (newBrightness < 0)
                                    newBrightness = 0;

                                if (newBrightness > 100)
                                    newBrightness = 100;

                                el.SetBrightnessSlow((uint)newBrightness);
                            }

                        }
                    }
                    catch
                    {
                        // ignored
                    }
                });


    }

    public static Rect? GetRectangle()
    {
        try
        {
            var rect = Shell32.GetNotifyIconRect(TrayIconGuid);

            return new Rect
            {
                X = rect.Left,
                Y = rect.Top,
                Width = rect.Right - rect.Left,
                Height = rect.Bottom - rect.Top
            };
        }
        catch
        {
            return null;
        }
    }

    public static void UpdateTheme(ApplicationTheme theme)
    {
        var factor = DpiTools.DpiFactorX;
        var iconSize = new Size((int)(SystemParameters.SmallIconWidth * factor),
            (int)(SystemParameters.SmallIconHeight * factor));

        IconInTray.Icon = new Icon(theme == ApplicationTheme.Light ? Resources.iconBlack : Resources.iconWhite,
            iconSize);
    }
}