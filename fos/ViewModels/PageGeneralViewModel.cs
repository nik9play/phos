using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using fos.Properties;
using fos.Tools;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;

namespace fos.ViewModels;

internal class Language
{
    public string Id { get; set; }
    public string Name { get; set; }
}

internal class PageGeneralViewModel : INotifyPropertyChanged
{
    private readonly RegistryKey _rkApp =
        Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

    private uint _allMonitorsbrightnessChangeInterval = SettingsController.Store.AllMonitorsBrightnessChangeInterval;

    private bool _autoCheckUpdates = SettingsController.Store.AutoUpdateCheckEnabled;
    private bool _autoStart;

    private uint _brightnessChangeInterval = SettingsController.Store.BrightnessChangeInterval;
    private uint _trayIconBrightnessChangeInterval = SettingsController.Store.TrayIconBrightnessChangeInterval;

    private bool _restartRequired;

    private string _selectedLanguage = SettingsController.Store.Language;

    public PageGeneralViewModel()
    {
        _autoStart = _rkApp.GetValue("phos") != null;
    }

    public RelayCommand RestartApplicationCommand => CommonCommands.RestartApplicationCommand;

    public bool AutoStart
    {
        get => _autoStart;

        set
        {
            _autoStart = value;
            if (value)
                _rkApp.SetValue("phos",
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "phos.exe"));
            else
                _rkApp.DeleteValue("phos");

            OnPropertyChanged();
        }
    }

    public uint BrightnessChangeInterval
    {
        get => _brightnessChangeInterval;
        set
        {
            _brightnessChangeInterval = value;
            SettingsController.Store.BrightnessChangeInterval = _brightnessChangeInterval;
            RestartRequired = true;
            OnPropertyChanged();
        }
    }

    public uint AllMonitorsBrightnessChangeInterval
    {
        get => _allMonitorsbrightnessChangeInterval;
        set
        {
            _allMonitorsbrightnessChangeInterval = value;
            SettingsController.Store.AllMonitorsBrightnessChangeInterval = _allMonitorsbrightnessChangeInterval;
            RestartRequired = true;
            OnPropertyChanged();
        }
    }

    public uint TrayIconBrightnessChangeInterval
    {
        get => _trayIconBrightnessChangeInterval;
        set
        {
            _trayIconBrightnessChangeInterval = value;
            SettingsController.Store.TrayIconBrightnessChangeInterval = _trayIconBrightnessChangeInterval;
            RestartRequired = true;
            OnPropertyChanged();
        }
    }

    public bool AutoCheckUpdates
    {
        get => _autoCheckUpdates;

        set
        {
            _autoCheckUpdates = value;
            SettingsController.Store.AutoUpdateCheckEnabled = value;

            if (value)
                UpdateManager.StartTimer();
            else
                UpdateManager.StopTimer();

            OnPropertyChanged();
        }
    }

    public ObservableCollection<Language> AvailableLanguages { get; } = GetAvailableLanguages();

    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            _selectedLanguage = value;
            SettingsController.Store.Language = value;
            RestartRequired = true;
            OnPropertyChanged();
        }
    }

    public bool RestartRequired
    {
        get => _restartRequired;
        set
        {
            _restartRequired = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public static ObservableCollection<Language> GetAvailableLanguages()
    {
        var languages = new ObservableCollection<Language>();
        languages.Add(new Language { Id = "system", Name = Resources.SettingsGeneralLanguageSystem });

        var rm = new ResourceManager(typeof(Resources));

        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        foreach (var culture in cultures)
            try
            {
                if (culture.Equals(CultureInfo.InvariantCulture))
                {
                    languages.Add(new Language { Id = "en", Name = "English - English [en]" });
                    continue;
                }

                var rs = rm.GetResourceSet(culture, true, false);
                if (rs != null)
                    languages.Add(new Language
                    {
                        Id = culture.Name, Name = $"{culture.NativeName} - {culture.EnglishName} [{culture.Name}]"
                    });
            }
            catch (CultureNotFoundException)
            {
            }

        return languages;
    }

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}