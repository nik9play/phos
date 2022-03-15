using fos.ViewModels;
using NHotkey;
using NHotkey.Wpf;

namespace fos
{
    internal static class HotkeysManager
    {
        public static void InitHotkeys()
        {
            if (!SettingsController.Store.HotkeysEnabled) return;
            try
            {
                var hotkeyUpKeys = new Keys(SettingsController.Store.HotkeyUp);
                HotkeyManager.Current.AddOrReplace("HotkeyUp", hotkeyUpKeys.GetGesture(), OnHotkey);
            }
            catch
            {
                HotkeyManager.Current.Remove("HotkeyUp");
            }


            try
            {
                var hotkeyDownKeys = new Keys(SettingsController.Store.HotkeyDown);
                HotkeyManager.Current.AddOrReplace("HotkeyDown", hotkeyDownKeys.GetGesture(), OnHotkey);
            }
            catch
            {
                HotkeyManager.Current.Remove("HotkeyDown");
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
                var multiplier = e.Name == "HotkeyUp" ? 1 : -1;
                var offset = multiplier * (int)SettingsController.Store.HotkeyStep;

                var currentMonitorInfo = MonitorTools.GetCurrentMonitor();

                if (SettingsController.Store.AllMonitorsModeEnabled)
                {
                    var newBrightness = (int)MainWindowViewModel.AllMonitorsBrightness + offset;

                    if (newBrightness < 0)
                        newBrightness = 0;

                    if (newBrightness > 100)
                        newBrightness = 100;

                    try
                    {
                        MainWindowViewModel.AllMonitorsBrightness = (uint)newBrightness;
                    }
                    catch
                    {
                        // ignored
                    }

                    WindowManager.HotkeyWindow.SetValue((uint)newBrightness);
                }
                else
                {
                    foreach (var el in MainWindowViewModel.Monitors)
                        if (el.Resolution.Width == (int)currentMonitorInfo.Resolution.Width &&
                            el.Resolution.Height == (int)currentMonitorInfo.Resolution.Height &&
                            el.Position.X == (int)currentMonitorInfo.Position.X &&
                            el.Position.Y == (int)currentMonitorInfo.Position.Y)
                        {
                            var newBrightness = (int)el.Brightness + offset;

                            if (newBrightness < 0)
                                newBrightness = 0;

                            if (newBrightness > 100)
                                newBrightness = 100;

                            try
                            {
                                el.Brightness = (uint)newBrightness;
                            }
                            catch
                            {
                                // ignored
                            }

                            WindowManager.HotkeyWindow.SetValue((uint)newBrightness);
                            break;
                        }
                }
            }

            e.Handled = true;
        }
    }
}