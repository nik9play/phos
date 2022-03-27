using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using fos.ViewModels;
using Page = ModernWpf.Controls.Page;

namespace fos.SettingsPages;

/// <summary>
///     Логика взаимодействия для Hotkeys.xaml
/// </summary>
public partial class Hotkeys : Page
{
    public Hotkeys()
    {
        InitializeComponent();
        DataContext = new PageHotkeysViewModel();
        //Properties.Resources.Sett
    }

    private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Command == ApplicationCommands.Copy ||
            e.Command == ApplicationCommands.Cut ||
            e.Command == ApplicationCommands.Paste)
            e.Handled = true;
    }

    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = true;

        var key = e.Key == Key.System ? e.SystemKey : e.Key;

        if (key == Key.LeftShift || key == Key.RightShift
                                 || key == Key.LeftCtrl || key == Key.RightCtrl
                                 || key == Key.LeftAlt || key == Key.RightAlt
                                 || key == Key.LWin || key == Key.RWin)
            return;

        ((TextBox)sender).Text = GetHotkey(key);

        FocusManager.SetFocusedElement(FocusManager.GetFocusScope((TextBox)sender), null);
        Keyboard.ClearFocus();
    }

    private string GetHotkey(Key key)
    {
        var HotkeyText = new StringBuilder();
        if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) HotkeyText.Append("Ctrl+");
        if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) HotkeyText.Append("Shift+");
        if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0) HotkeyText.Append("Alt+");
        HotkeyText.Append(key.ToString());

        return HotkeyText.ToString();
    }
}