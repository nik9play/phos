using System;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using ModernWpf;

namespace fos;

public enum OSVersions
{
    RS3 = 16299,
    RS4 = 17134,
    RS5_1809 = 17763
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
    public ThemeChangingArgs(ApplicationTheme theme)
    {
        CurrentTheme = theme;
        //CurrentAccentColor = color;
    }

    public ApplicationTheme CurrentTheme { get; }
    //public Color CurrentAccentColor { get; private set; }
}

public static class ThemeTools
{
    private static readonly string s_PersonalizeKey =
        @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

    private static readonly string s_AccentColorKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Accent";

    private static readonly DispatcherTimer
        ThemeChangeTimer = new() { Interval = TimeSpan.FromMilliseconds(2000) };

    private static ApplicationTheme LastCurrentTheme;

    static ThemeTools()
    {
        ThemeChangeTimer.Tick += ThemeChangeTimer_Tick;
        ThemeChangeTimer.Start();
    }

    public static ApplicationTheme CurrentTheme => GetTheme();
    public static Color CurrentAccentColor => GetColor();

    private static Color GetColor()
    {
        var color = ReadDword(s_AccentColorKey, "AccentColorMenu");

        var newColor = Color.FromRgb((byte)(color & 0x000000ff), (byte)((color & 0x0000ff00) / 0xff),
            (byte)((color & 0x00ff0000) / 0xffff));
        return newColor;
    }

    private static ApplicationTheme GetTheme()
    {
        if (LightThemeShim(ReadDwordBool(s_PersonalizeKey, "SystemUsesLightTheme")))
            return ApplicationTheme.Light;
        return ApplicationTheme.Dark;
    }

    private static uint ReadDword(string key, string valueName, int defaultValue = 0)
    {
        using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
        using (var subKey = baseKey.OpenSubKey(key))
        {
            var toReturn = subKey.GetValue(valueName, defaultValue);

            if (toReturn is int)
                return BitConverter.ToUInt32(BitConverter.GetBytes((int)toReturn), 0);
            return 0;
        }
    }

    private static bool ReadDwordBool(string key, string valueName, int defaultValue = 0)
    {
        using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
        using (var subKey = baseKey.OpenSubKey(key))
        {
            return (int)subKey.GetValue(valueName, defaultValue) > 0;
        }
    }

    private static bool LightThemeShim(bool registryValue)
    {
        return Environment.OSVersion.IsGreaterThan(OSVersions.RS5_1809) && registryValue;
    }

    public static event EventHandler<ThemeChangingArgs> ThemeChanged = delegate { };
    //private static Color LastAccentColor;

    private static void ThemeChangeTimer_Tick(object sender, EventArgs e)
    {
        CheckTheme();
    }

    private static void CheckTheme()
    {
        var fireEvent = false;

        var newCurrentTheme = CurrentTheme;

        if (newCurrentTheme != LastCurrentTheme) fireEvent = true;

        LastCurrentTheme = newCurrentTheme;

        //Color NewAccentColor = CurrentAccentColor;

        //if (NewAccentColor != LastAccentColor)
        //{
        //    fireEvent = true;
        //}

        //LastAccentColor = NewAccentColor;

        if (fireEvent)
            ThemeChanged(null, new ThemeChangingArgs(newCurrentTheme));
    }
}