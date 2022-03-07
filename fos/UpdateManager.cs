using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using fos.Extensions;
using Microsoft.Toolkit.Uwp.Notifications;

namespace fos
{
    class UpdateCheckResult
    {
        public bool UpdateAvailable { get; set; }
        public string LatestVersionUrl { get; set; }
        public string LatestChangeLog { get; set; }
        public Version LatestVersion { get; set; }
    }

    class AssetInfo
    {
        public string browser_download_url { get; set; }
    }

    class ApiResponse
    {
        public string tag_name { get; set; }
        public AssetInfo[] assets { get; set; }
        public string body { get; set; }
    }

    static class UpdateManager
    {
        static readonly string apiUrl = "http://localhost:8000/huita.json";

        private static readonly HttpClient client = new HttpClient();

        static UpdateManager()
        {
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Add("user-agent", "request");
        }

        public static async Task<UpdateCheckResult> CheckUpdates()
        {
            using (var response = await client.GetAsync(apiUrl))
            {
                using (var content = response.Content)
                {
                    var json = await content.ReadAsStreamAsync();

                    ApiResponse result = await JsonSerializer.DeserializeAsync<ApiResponse>(json);
                    Version version = new Version(result.tag_name);
                    string downloadUrl = result.assets[0].browser_download_url;
                    string changeLog = result.body;

                    Version currentVersion = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);

                    bool updateAvailable = false;

                    if (version.CompareTo(currentVersion) > 0)
                    {
                        updateAvailable = true;
                    }

                    return new UpdateCheckResult
                    { 
                        UpdateAvailable = updateAvailable,
                        LatestVersionUrl = downloadUrl,
                        LatestChangeLog = changeLog,
                        LatestVersion = version
                    };
                }
            }
        }

        public static async Task Update(UpdateCheckResult updateCheckResult, IProgress<float> progress, CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "phos.updates"));
            string fileName = Path.Combine(Path.GetTempPath(), "phos.updates", "phos_update_" + Guid.NewGuid().ToString() + ".exe");

            using (var fileStream = new FileStream(fileName, FileMode.CreateNew))
            {
                await client.DownloadAsync(updateCheckResult.LatestVersionUrl, fileStream, progress, cancellationToken);
            }

            Process process = new Process();

            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = "/SILENT";
            process.StartInfo.UseShellExecute = true;

            process.Start();
            Application.Current.Shutdown();
        }

        static private DispatcherTimer checkUpdateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromHours(2)
        };

        static public void StartTimer()
        {
            checkUpdateTimer.Start();
        }

        static public void StopTimer()
        {
            checkUpdateTimer.Stop();
        }

        static public async Task CheckUpdatesSilent()
        {
            try
            {
                UpdateCheckResult updateCheckResult = await CheckUpdates();

                if (updateCheckResult.UpdateAvailable)
                {
                    new ToastContentBuilder()
                        .AddArgument("action", "update")
                        .AddText(Properties.Resources.SettingsAboutUpdateAvailable)
                        .AddText(Properties.Resources.SettingsAboutVersion + updateCheckResult.LatestVersion)
                        .Show();
                }
            }
            catch { }
        }

        static public void InitTimer()
        {
            checkUpdateTimer.Tick += new EventHandler(async (object s, EventArgs a) => await CheckUpdatesSilent());
        }
    }
}
