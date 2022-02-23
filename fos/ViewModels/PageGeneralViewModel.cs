using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Resources;
using System.Collections.ObjectModel;

namespace fos.ViewModels
{
    class Language
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    class PageGeneralViewModel : INotifyPropertyChanged
    { 
        private RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private bool autoStart;
        public bool AutoStart
        {
            get { return autoStart; }

            set
            {
                autoStart = value;
                if (value)
                    rkApp.SetValue("fos", System.Reflection.Assembly.GetExecutingAssembly().Location);
                else
                    rkApp.DeleteValue("fos");

                OnPropertyChanged();
            }
        }

        private uint brightnessChangeInterval = SettingsController.Store.BrightnessChangeInterval;
        public uint BrightnessChangeInterval
        {
            get { return brightnessChangeInterval; }
            set
            {
                brightnessChangeInterval = value;
                SettingsController.Store.BrightnessChangeInterval = brightnessChangeInterval;
                RestartRequired = true;
                OnPropertyChanged();
            }
        }

        private uint allMonitorsbrightnessChangeInterval = SettingsController.Store.AllMonitorsBrightnessChangeInterval;
        public uint AllMonitorsBrightnessChangeInterval
        {
            get { return allMonitorsbrightnessChangeInterval; }
            set
            {
                allMonitorsbrightnessChangeInterval = value;
                SettingsController.Store.AllMonitorsBrightnessChangeInterval = allMonitorsbrightnessChangeInterval;
                RestartRequired = true;
                OnPropertyChanged();
            }
        }

        private bool autoCheckUpdates = SettingsController.Store.AutoUpdateCheckEnabled;
        public bool AutoCheckUpdates
        {
            get { return autoCheckUpdates; }

            set 
            {
                autoCheckUpdates = value;
                SettingsController.Store.AutoUpdateCheckEnabled = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Language> AvailableLanguages { get; } = GetAvailableLanguages();

        public PageGeneralViewModel()
        {
            autoStart = rkApp.GetValue("fos") != null;
        }

        private string selectedLanguage = SettingsController.Store.Language;
        public string SelectedLanguage
        {
            get { return selectedLanguage; }
            set { selectedLanguage = value; SettingsController.Store.Language = value; RestartRequired = true; OnPropertyChanged(); }
        }

        private bool restartRequired = false;
        public bool RestartRequired
        {
            get { return restartRequired; }
            set { restartRequired = value; OnPropertyChanged();}
        }

        public static ObservableCollection<Language> GetAvailableLanguages()
        {
            var languages = new ObservableCollection<Language>();
            languages.Add(new Language() { Id = "system", Name = Properties.Resources.SettingsGeneralLanguageSystem });

            ResourceManager rm = new ResourceManager(typeof(Properties.Resources));

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo culture in cultures)
            {
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture))
                    {
                        languages.Add(new Language() { Id = "en", Name = "English - English [en]" });
                        continue;
                    }

                    ResourceSet rs = rm.GetResourceSet(culture, true, false);
                    if (rs != null)
                        languages.Add(new Language() { Id = culture.Name, Name = $"{culture.NativeName} - {culture.EnglishName} [{culture.Name}]" });
                }
                catch (CultureNotFoundException) { }
            }
            return languages;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
