using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using fos.Properties;
using fos.Tools;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using ModernWpf.Controls;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using ValidationError = NJsonSchema.Validation.ValidationError;

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

        ImportCommand = new(() =>
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Config file|*.json";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                if (File.Exists(fileName))
                {
                    var json = File.ReadAllText(fileName);

                    var schema = JsonSchema.FromType<Settings>(new JsonSchemaGeneratorSettings { AlwaysAllowAdditionalObjectProperties = false });

                    try
                    {
                        List<ValidationError> errors = schema.Validate(json).ToList();

                        if (errors.Count > 0)
                        {
                            var errorStringBuilder = new StringBuilder();

                            foreach (var error in errors)
                            {
                                errorStringBuilder.Append($"{error}\n");
                            }

                            ShowError(errorStringBuilder.ToString());
                            return;
                        }

                        var settings = JsonConvert.DeserializeObject<Settings>(json);

                        if (settings == null)
                        {
                            ShowError("Settings is null");
                            return;
                        }

                        settings.FirstStart = false;

                        SettingsController.Store = settings;

                        SettingsController.SaveSettings();

                        ShowMessage();
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                        return;
                    }

                }
            }
        });

        ExportCommand = new(() =>
        {
            var openFileDialog = new SaveFileDialog();
            openFileDialog.Filter = "Config file|*.json";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                string directoryName = Path.GetDirectoryName(openFileDialog.FileName);
                
                try
                {
                    Directory.CreateDirectory(directoryName!);
                    File.WriteAllText(fileName,
                        JsonConvert.SerializeObject(SettingsController.Store, Formatting.Indented));
                }
                catch
                {
                    ShowError("Сука.");
                }
            }
        });
    }

    public void ShowError(string message)
    {
        _ = new ContentDialog
        {
            Title = Properties.Resources.SettingsBackupErrorTitle,
            Content = message,
            CloseButtonText = Properties.Resources.CloseButton
        }.ShowAsync();
    }

    public void ShowMessage()
    {
        _ = new ContentDialog
        {
            Title = Properties.Resources.SettingsBackupSuccessTitle,
            Content = Properties.Resources.SettingsBackupSuccessDescription,
            PrimaryButtonText = Properties.Resources.RestartButton,
            PrimaryButtonCommand = CommonCommands.RestartApplicationCommand,

        }.ShowAsync();
    }

    public RelayCommand RestartApplicationCommand => CommonCommands.RestartApplicationCommand;

    public RelayCommand ImportCommand { get; }
    public RelayCommand ExportCommand { get; }

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

    public bool IsContainerized => PackageHelper.IsContainerized();

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