using System;
using System.Windows.Input;

namespace fos;

internal class Keys
{
    public Key MainKey;
    public ModifierKeys Modifiers;

    public Keys(string hotkeyString)
    {
        var hotkeyArray = hotkeyString.Split('+');
        foreach (var element in hotkeyArray)
            switch (element)
            {
                case "Ctrl":
                    Modifiers |= ModifierKeys.Control;
                    break;
                case "Alt":
                    Modifiers |= ModifierKeys.Alt;
                    break;
                case "Shift":
                    Modifiers |= ModifierKeys.Shift;
                    break;
                default:
                    Enum.TryParse(element, out MainKey);
                    break;
            }
    }

    public KeyGesture GetGesture()
    {
        return new KeyGesture(MainKey, Modifiers);
    }
}