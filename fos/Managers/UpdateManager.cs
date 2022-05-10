using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using fos.Extensions;
using fos.Properties;
using fos.Tools;
using Microsoft.Toolkit.Uwp.Notifications;
using Octokit;
using Application = System.Windows.Application;
using FileMode = System.IO.FileMode;

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
    private static readonly HttpClient HttpClient = new();
    private static readonly GitHubClient GitHubClient = new(new ProductHeaderValue("phos"));

    private static readonly DispatcherTimer CheckUpdateTimer = new()
    {
        Interval = TimeSpan.FromHours(2)
    };

    static UpdateManager()
    {
        HttpClient.Timeout = TimeSpan.FromSeconds(15);
        HttpClient.DefaultRequestHeaders.Add("user-agent", "request");
    }

    public static UpdateCheckResult LatestUpdateCheckResult { get; private set; }

    private static string GetInstaller(IEnumerable<ReleaseAsset> assetsList)
    {
        var regex = new Regex("^phos_setup.+\\.exe$");
        var result = assetsList.Where(el => regex.IsMatch(el.Name));

        return result.FirstOrDefault()?.BrowserDownloadUrl;
    }

    public static async Task<UpdateCheckResult> CheckUpdates()
    {
        var releases = await GitHubClient.Repository.Release.GetAll("nik9play", "phos");
        var latest = releases[0];
        var downloadUrl = GetInstaller(latest.Assets);

        var latestVersion = new Version(latest.TagName);
        var currentVersion =
            new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion!);

        LatestUpdateCheckResult = new UpdateCheckResult
        {
            UpdateAvailable = latestVersion > currentVersion,
            LatestVersionUrl = downloadUrl,
            LatestChangeLog = latest.Body,
            LatestVersion = latestVersion
        };

        return LatestUpdateCheckResult;
    }

    public static async Task Update(UpdateCheckResult updateCheckResult, IProgress<float> progress,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "phos.updates"));
        var fileName = Path.Combine(Path.GetTempPath(), "phos.updates",
            "phos_update_" + Guid.NewGuid() + ".exe");

        await using (var fileStream = new FileStream(fileName, FileMode.CreateNew))
        {
            await HttpClient.DownloadAsync(updateCheckResult.LatestVersionUrl, fileStream, progress, cancellationToken);
        }

        var process = new Process();

        process.StartInfo.FileName = fileName;
        process.StartInfo.Arguments = "/SILENT";
        process.StartInfo.UseShellExecute = true;

        AppMutex.UnrealeseMutex();

        process.Start();

        Application.Current.Shutdown();
    }

    public static void StartTimer()
    {
        if (PackageHelper.IsContainerized()) return;
        CheckUpdateTimer.Start();
    }

    public static void StopTimer()
    {
        CheckUpdateTimer.Stop();
    }

    public static async Task CheckUpdatesSilent()
    {
        if (PackageHelper.IsContainerized()) return;

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
        if (PackageHelper.IsContainerized()) return;
        CheckUpdateTimer.Tick += async (_, _) => await CheckUpdatesSilent();
    }
}