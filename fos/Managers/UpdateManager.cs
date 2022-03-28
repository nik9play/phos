using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using fos.Extensions;
using fos.Properties;
using Microsoft.Toolkit.Uwp.Notifications;

namespace fos;

public class UpdateCheckResult
{
    public bool UpdateAvailable { get; set; }
    public string LatestVersionUrl { get; set; }
    public string LatestChangeLog { get; set; }
    public Version LatestVersion { get; set; }
}

public class AssetInfo
{
    public string browser_download_url { get; set; }
}

public class ApiResponse
{
    public string tag_name { get; set; }
    public AssetInfo[] assets { get; set; }
    public string body { get; set; }
}

public static class UpdateManager
{
#if DEBUG
    private const string ApiUrl = "http://localhost:8000/huita.json";
#else
        private const string ApiUrl = "https://api.github.com/repos/nik9play/phos/releases/latest";
#endif

    private static readonly HttpClient client = new();

    static UpdateManager()
    {
        client.Timeout = TimeSpan.FromSeconds(15);
        client.DefaultRequestHeaders.Add("user-agent", "request");
    }

    public static async Task<UpdateCheckResult> CheckUpdates()
    {
        using (var response = await client.GetAsync(ApiUrl))
        {
            using (var content = response.Content)
            {
                var json = await content.ReadAsStreamAsync();

                var result = await JsonSerializer.DeserializeAsync<ApiResponse>(json);
                var version = new Version(result.tag_name);
                var downloadUrl = result.assets[1].browser_download_url;
                var changeLog = result.body;

                var currentVersion = new Version(FileVersionInfo
                    .GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);

                var updateAvailable = version.CompareTo(currentVersion) > 0;

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

    public static async Task Update(UpdateCheckResult updateCheckResult, IProgress<float> progress,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "phos.updates"));
        var fileName = Path.Combine(Path.GetTempPath(), "phos.updates",
            "phos_update_" + Guid.NewGuid() + ".exe");

        await using (var fileStream = new FileStream(fileName, FileMode.CreateNew))
        {
            await client.DownloadAsync(updateCheckResult.LatestVersionUrl, fileStream, progress, cancellationToken);
        }

        var process = new Process();

        process.StartInfo.FileName = fileName;
        process.StartInfo.Arguments = "/SILENT";
        process.StartInfo.UseShellExecute = true;

        process.Start();
        Application.Current.Shutdown();
    }

    private static readonly DispatcherTimer CheckUpdateTimer = new()
    {
        Interval = TimeSpan.FromHours(2)
    };

    public static void StartTimer()
    {
        CheckUpdateTimer.Start();
    }

    public static void StopTimer()
    {
        CheckUpdateTimer.Stop();
    }

    public static async Task CheckUpdatesSilent()
    {
        try
        {
            var updateCheckResult = await CheckUpdates();

            if (updateCheckResult.UpdateAvailable)
                new ToastContentBuilder()
                    .AddArgument("action", "update")
                    .AddText(Resources.SettingsAboutUpdateAvailable)
                    .AddText(Resources.SettingsAboutVersion + updateCheckResult.LatestVersion)
                    .Show();
        }
        catch
        {
            // ignored
        }
    }

    public static void InitTimer()
    {
        CheckUpdateTimer.Tick += async (_, _) => await CheckUpdatesSilent();
    }
}