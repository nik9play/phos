using System;
using System.Windows.Media;

namespace fos.Tools;

public static class PlatformTools
{
    public static bool IsWindows11() => Environment.OSVersion.IsAtLeast(OSVersions.WIN11_21H2);

    public static FontFamily MDL2Assets = new("Segoe MDL2 Assets");
    public static FontFamily FluentIcons = new("Segoe Fluent Icons");

    public static FontFamily GetSymbolFont() => IsWindows11() ? FluentIcons : MDL2Assets;
}