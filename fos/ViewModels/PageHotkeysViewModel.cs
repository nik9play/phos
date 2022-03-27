using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace fos.ViewModels;

internal class PageHotkeysViewModel : INotifyPropertyChanged
{
    private string _hotkeyDown = SettingsController.Store.HotkeyDown;

    private HotkeyPopupLocationEnum _hotkeyPopupLocation = SettingsController.Store.HotkeyPopupLocation;
    private bool _hotkeysEnabled = SettingsController.Store.HotkeysEnabled;

    private uint _hotkeyStep = SettingsController.Store.HotkeyStep;

    private string _hotkeyUp = SettingsController.Store.HotkeyUp;

    public bool HotkeysEnabled
    {
        get => _hotkeysEnabled;

        set
        {
            _hotkeysEnabled = value;
            SettingsController.Store.HotkeysEnabled = value;

            if (value) HotkeysManager.InitHotkeys();
            else HotkeysManager.RemoveHotkeys();

            OnPropertyChanged();
        }
    }

    public string HotkeyUp
    {
        get => _hotkeyUp;

        set
        {
            _hotkeyUp = value;
            SettingsController.Store.HotkeyUp = value;

            if (string.IsNullOrEmpty(value))
            {
                _hotkeyUp = SettingsController.DefaultSettings.HotkeyUp;
                SettingsController.Store.HotkeyUp = SettingsController.DefaultSettings.HotkeyUp;
            }

            HotkeysManager.RemoveHotkeys();
            HotkeysManager.InitHotkeys();

            OnPropertyChanged();
        }
    }

    public string HotkeyDown
    {
        get => _hotkeyDown;

        set
        {
            _hotkeyDown = value;
            SettingsController.Store.HotkeyDown = value;

            if (string.IsNullOrEmpty(value))
            {
                _hotkeyDown = SettingsController.DefaultSettings.HotkeyDown;
                SettingsController.Store.HotkeyDown = SettingsController.DefaultSettings.HotkeyDown;
            }

            HotkeysManager.RemoveHotkeys();
            HotkeysManager.InitHotkeys();

            OnPropertyChanged();
        }
    }

    public uint HotkeyStep
    {
        get => _hotkeyStep;

        set
        {
            _hotkeyStep = value;
            SettingsController.Store.HotkeyStep = value;

            OnPropertyChanged();
        }
    }

    public HotkeyPopupLocationEnum HotkeyPopupLocation
    {
        get => _hotkeyPopupLocation;
        set
        {
            _hotkeyPopupLocation = value;
            SettingsController.Store.HotkeyPopupLocation = value;

            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}