using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace fos
{
    class Keys
    {
        public ModifierKeys Modifiers;
        public Key MainKey;

        public KeyGesture GetGesture()
        {
            return new KeyGesture(MainKey, Modifiers);
        }

        public Keys(string HotkeyString)
        {
            string[] HotkeyArray = HotkeyString.Split('+');
            foreach (string Element in HotkeyArray)
            {
                switch (Element)
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
                        Enum.TryParse(Element, out MainKey);
                        break;
                }
            }
        }
    }
}
