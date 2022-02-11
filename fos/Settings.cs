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
    }
}
