using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

namespace fos
{
    public static class SettingsController
    {
        private static readonly string _configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");
        public static Settings Store { get; private set; }
        public static readonly Settings defaultSettings = new Settings();

        private static readonly JsonSerializerOptions _options;

        static SettingsController()
        {
            _options = new JsonSerializerOptions
            {
                Converters = {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                },
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        public static void LoadSettings()
        {
            if (File.Exists(_configPath))
            {
                string json = File.ReadAllText(_configPath);
                try
                {
                    Store = JsonSerializer.Deserialize<Settings>(json, _options);
                }
                catch
                {
                    Store = new Settings();

                    new ToastContentBuilder()
                        .AddText(Properties.Resources.LoadSettingsErrorTitle)
                        .AddText(Properties.Resources.LoadSettingsErrorDescription)
                        .Show();
                }
            }
            else
            {
                Store = new Settings();
                try
                {
                    SaveSettings();
                }
                catch
                {
                    new ToastContentBuilder()
                        .AddText(Properties.Resources.LoadSettingsErrorTitle)
                        .AddText(Properties.Resources.LoadSettingsErrorDescription)
                        .Show();
                }
            }
        }

        public static void LoadLanguage()
        {
            CultureInfo language;
            switch (Store.Language)
            {
                case "ru":
                    language = new CultureInfo(Store.Language);
                    break;
                case "en":
                    language = CultureInfo.InvariantCulture;
                    break;
                case "system":
                default:
                    language = CultureInfo.InstalledUICulture;
                    break;
            }

            Properties.Resources.Culture = language;
        }

        public static void SaveSettings()
        {
            File.WriteAllText(_configPath, JsonSerializer.Serialize<Settings>(Store, _options));
        }
    }
}
