using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using fos.Properties;
using fos.Tools;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;

namespace fos;

public static class SettingsController
{
    private static readonly string ConfigPath;

    public static readonly Settings DefaultSettings = new();

    //private static readonly JsonSerializerOptions _options;

    static SettingsController()
    {
        //_options = new JsonSerializerOptions
        //{
        //    Converters = {
        //        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        //    },
        //    WriteIndented = true,
        //    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        //};

        if (PackageHelper.IsContainerized())
            ConfigPath =
                Path.Combine(Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData), "phos/config.json"));
        else
            ConfigPath =
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "config.json");
    }

    public static Settings Store { get; private set; }

    public static void LoadSettings()
    {
        if (File.Exists(ConfigPath))
        {
            var json = File.ReadAllText(ConfigPath);
            try
            {
                var schema = JsonSchema.FromType<Settings>(new JsonSchemaGeneratorSettings { AlwaysAllowAdditionalObjectProperties = true });

                var errors = schema.Validate(json);

                //foreach (var error in errors)
                //    Debug.WriteLine(error.Path + ": " + error.Kind);

                if (errors.Count > 0)
                {
                    Store = new Settings();

                    new ToastContentBuilder()
                        .AddText(Resources.SettingsSchemaErrorTitle)
                        .AddText(Resources.SettingsSchemaErrorDescription)
                        .Show();

                    return;
                }

                Store = JsonConvert.DeserializeObject<Settings>(json);
            }
            catch
            {
                Store = new Settings();

                new ToastContentBuilder()
                    .AddText(Resources.LoadSettingsErrorTitle)
                    .AddText(Resources.LoadSettingsErrorDescription)
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
                    .AddText(Resources.LoadSettingsErrorTitle)
                    .AddText(Resources.LoadSettingsErrorDescription)
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

        Thread.CurrentThread.CurrentCulture = language;
        Thread.CurrentThread.CurrentUICulture = language;
        CultureInfo.DefaultThreadCurrentCulture = language;
        CultureInfo.DefaultThreadCurrentUICulture = language;
    }

    public static void SaveSettings()
    {
        if (!File.Exists(ConfigPath))
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);

        File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Store, Formatting.Indented));
    }

    public static void OpenSettingsFolder()
    {
        if (!File.Exists(ConfigPath)) return;

        var argument = "/select, \"" + ConfigPath + "\"";

        Process.Start("explorer.exe", argument);
    }
}