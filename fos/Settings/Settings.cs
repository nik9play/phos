using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace fos;

[JsonConverter(typeof(StringEnumConverter))]
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
    public bool FirstStart { get; set; } = true;
    public bool AllMonitorsModeEnabled { get; set; } = false;
    public string Language { get; set; } = "system";
    public bool AutoUpdateCheckEnabled { get; set; } = false;
    public bool HotkeysEnabled { get; set; } = false;
    public string HotkeyUp { get; set; } = "Alt+F2";
    public string HotkeyDown { get; set; } = "Alt+F1";

    [Range(1, 25)] public uint HotkeyStep { get; set; } = 5;

    public HotkeyPopupLocationEnum HotkeyPopupLocation { get; set; } = HotkeyPopupLocationEnum.BottomCenter;

    [Range(10, 1000)] public uint BrightnessChangeInterval { get; set; } = 50;

    [Range(10, 1000)] public uint AllMonitorsBrightnessChangeInterval { get; set; } = 100;

    public List<string> MonitorListLocationOverwrites { get; set; } = new();
    public Dictionary<string, MonitorCustomLimits> MonitorCustomLimits { get; set; } = new();
}