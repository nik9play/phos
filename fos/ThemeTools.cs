using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using ModernWpf;
using System.Windows.Media;
using System.Diagnostics;

namespace fos
{
    public enum OSVersions : int
    {
        RS3 = 16299,
        RS4 = 17134,
        RS5_1809 = 17763,
    }

    public static class OperatingSystemExtensions
    {
        public static bool IsAtLeast(this OperatingSystem os, OSVersions version)
        {
            return os.Version.Build >= (int)version;
        }

        public static bool IsGreaterThan(this OperatingSystem os, OSVersions version)
        {
            return os.Version.Build > (int)version;
        }
    }

    public class ThemeChangingArgs : EventArgs
    {
        public ApplicationTheme CurrentTheme { get; private set; }
        public Color CurrentAccentColor { get; private set; }

        public ThemeChangingArgs(ApplicationTheme theme)
        {
            CurrentTheme = theme;
            //CurrentAccentColor = color;
        }
    }

    public static class ThemeTools
    {
        static readonly string s_PersonalizeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        static readonly string s_AccentColorKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Accent";

        public static ApplicationTheme CurrentTheme => GetTheme();
        public static Color CurrentAccentColor => GetColor();

        private static Color GetColor()
        {
            uint color = ReadDword(s_AccentColorKey, "AccentColorMenu");

            Color newColor = Color.FromRgb((byte)(color & 0x000000ff), (byte)((color & 0x0000ff00) / 0xff), (byte)((color & 0x00ff0000) / 0xffff));
            return newColor;
        }

        private static ApplicationTheme GetTheme()
        {
            if (LightThemeShim(ReadDwordBool(s_PersonalizeKey, "SystemUsesLightTheme")))
                return ApplicationTheme.Light;
            else
                return ApplicationTheme.Dark;
        }

        private static readonly DispatcherTimer _themeChangeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(2000) };

        static ThemeTools()
        {
            _themeChangeTimer.Tick += ThemeChangeTimer_Tick;
            _themeChangeTimer.Start();
        }

        private static uint ReadDword(string key, string valueName, int defaultValue = 0)
        {
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
            using (var subKey = baseKey.OpenSubKey(key))
            {
                var toReturn = subKey.GetValue(valueName, defaultValue);

                if (toReturn is int)
                {
                    return BitConverter.ToUInt32(BitConverter.GetBytes((int)toReturn), 0);
                }
                else
                    return 0;
            }
        }

        private static bool ReadDwordBool(string key, string valueName, int defaultValue = 0)
        {
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
            using (var subKey = baseKey.OpenSubKey(key))
            {
                return (int)subKey.GetValue(valueName, defaultValue) > 0;
            }
        }

        private static bool LightThemeShim(bool registryValue)
        {
            if (Environment.OSVersion.IsGreaterThan(OSVersions.RS5_1809))
            {
                return registryValue;
            }
            return false;
        }

        public static event EventHandler<ThemeChangingArgs> ThemeChanged = delegate { };

        private static ApplicationTheme LastCurrentTheme;
        //private static Color LastAccentColor;

        private static void ThemeChangeTimer_Tick(object sender, EventArgs e)
        {
            CheckTheme();
        }

        private static void CheckTheme()
        {
            bool fireEvent = false;

            ApplicationTheme NewCurrentTheme = CurrentTheme;

            if (NewCurrentTheme != LastCurrentTheme)
            {
                fireEvent = true;
            }

            LastCurrentTheme = NewCurrentTheme;

            //Color NewAccentColor = CurrentAccentColor;

            //if (NewAccentColor != LastAccentColor)
            //{
            //    fireEvent = true;
            //}

            //LastAccentColor = NewAccentColor;

            if (fireEvent)
                ThemeChanged(null, new ThemeChangingArgs(NewCurrentTheme));
        }
    }
}
