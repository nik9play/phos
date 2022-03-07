using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NHotkey;
using NHotkey.Wpf;

namespace fos
{
    static class HotkeysManager
    {
        public static void InitHotkeys()
        {
            if (SettingsController.Store.HotkeysEnabled)
            {
                try
                {
                    var HotkeyUpKeys = new Keys(SettingsController.Store.HotkeyUp);
                    HotkeyManager.Current.AddOrReplace("HotkeyUp", HotkeyUpKeys.GetGesture(), OnHotkey);
                }
                catch
                {
                    HotkeyManager.Current.Remove("HotkeyUp");
                }


                try
                {
                    var HotkeyDownKeys = new Keys(SettingsController.Store.HotkeyDown);
                    HotkeyManager.Current.AddOrReplace("HotkeyDown", HotkeyDownKeys.GetGesture(), OnHotkey);
                }
                catch
                {
                    HotkeyManager.Current.Remove("HotkeyDown");
                }
            }    
        }

        public static void RemoveHotkeys()
        {
            HotkeyManager.Current.Remove("HotkeyUp");
            HotkeyManager.Current.Remove("HotkeyDown");
        }

        private static void OnHotkey(object sender, HotkeyEventArgs e)
        {
            if (SettingsController.Store.HotkeysEnabled)
            {
                MonitorTools.MonitorInfo monitorInfo = MonitorTools.GetCurrentMonitor();
                Debug.WriteLine(monitorInfo.Position);

                int multiplier = (e.Name == "HotkeyUp") ? 1 : -1;
                int offset = multiplier * (int)SettingsController.Store.HotkeyStep;

                var currentMonitorInfo = MonitorTools.GetCurrentMonitor();

                if (SettingsController.Store.AllMonitorsModeEnabled)
                {
                    int newBrightness = (int)ViewModels.MainWindowViewModel.AllMonitorsBrightness + offset;

                    if (newBrightness < 0)
                        newBrightness = 0;

                    if (newBrightness > 100)
                        newBrightness = 100;

                    try { ViewModels.MainWindowViewModel.AllMonitorsBrightness = (uint)newBrightness; } catch { }
                    WindowManager.hotkeyWindow.SetValue((uint)newBrightness);
                }
                else
                {
                    foreach (var el in ViewModels.MainWindowViewModel.Monitors)
                    {
                        if (el.Resolution.Width == currentMonitorInfo.Resolution.Width &&
                            el.Resolution.Height == currentMonitorInfo.Resolution.Height &&
                            el.Position.X == currentMonitorInfo.Position.X &&
                            el.Position.Y == currentMonitorInfo.Position.Y)
                        {
                            int newBrightness = (int)el.Brightness + offset;

                            if (newBrightness < 0)
                                newBrightness = 0;

                            if (newBrightness > 100)
                                newBrightness = 100;

                            try { el.Brightness = (uint)newBrightness; } catch { }
                            WindowManager.hotkeyWindow.SetValue((uint)newBrightness);
                            break;
                        }
                    }
                }

                //if (WindowManager.hotkeyWindow.Visibility == Visibility.Visible)
                //{
                //    uint newBrightness = 0;

                //    if (_contoller.Brightness + offset < 0)
                //    {
                //        newBrightness = 0;
                //    }
                //    if (_contoller.Brightness + offset > 0)
                //    {
                //        newBrightness = 100;
                //    }

                //    _contoller.SetBrightness(newBrightness);
                //}
            }

            e.Handled = true;
        }
    }
}
