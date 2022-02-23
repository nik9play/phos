using System.Collections.Generic;

namespace fos
{
    public enum HotkeyPopupLocationEnum
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        BottomCenter,
        TopCenter
    }

    public class MonitorCustomLimits
    {
        public int Minimum { get; set; } = 0;
        public int Maximum { get; set; } = 100;
    }

    public class Settings
    {
        public bool AllMonitorsModeEnabled { get; set; } = false;
        public string Language { get; set; } = "system";
        public bool AutoUpdateCheckEnabled { get; set; } = false;
        public bool HotkeysEnabled { get; set; } = false;
        public string HotkeyUp { get; set; } = "Alt+F2";
        public string HotkeyDown { get; set; } = "Alt+F1";
        public uint HotkeyStep { get; set; } = 5;
        public HotkeyPopupLocationEnum HotkeyPopupLocation { get; set; } = HotkeyPopupLocationEnum.BottomCenter;
        public uint BrightnessChangeInterval { get; set; } = 50;
        public uint AllMonitorsBrightnessChangeInterval { get; set; } = 100;

        public List<string> MonitorListLocationOverwrites { get; set; } = new List<string>();
        public Dictionary<string, MonitorCustomLimits> MonitorCustomLimits { get; set; } = new Dictionary<string, MonitorCustomLimits>();
    }
}
