﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using fos.Properties;
using fos.Tools;
using Microsoft.Toolkit.Mvvm.Input;
using Octokit;

namespace fos.ViewModels;

internal class PageAboutViewModel : INotifyPropertyChanged
{
    private static bool updateChecking;

    private static UpdateCheckResult checkResult;

    private static bool isError;

    private static string errorMessage;

    private static string updatesText = Resources.SettingsAboutNoUpdates;

    private static bool updateAvailable;

    private static float progress;

    private static int progressPercent;

    private static bool updateInstalling;

    private static CancellationTokenSource cancelTokenSource;

    public PageAboutViewModel()
    {
        CheckUpdatesCommand = new AsyncRelayCommand(CheckUpdates);
        UpdateCommand = new AsyncRelayCommand(Update);
        CancelCommand = new RelayCommand(() => cancelTokenSource.Cancel());
        OpenStoreCommand =
            new RelayCommand(() =>
                Process.Start(new ProcessStartInfo("ms-windows-store://pdp/?productid=9NJGMVXZMB4M")
                    { UseShellExecute = true }));

        if (UpdateManager.LatestUpdateCheckResult != null)
        {
            CheckResult = UpdateManager.LatestUpdateCheckResult;

            UpdateAvailable = UpdateManager.LatestUpdateCheckResult.UpdateAvailable;

            if (CheckResult.UpdateAvailable)
            {
                UpdatesText = Resources.SettingsAboutUpdateAvailable;
                UpdateAvailable = true;
            }
            else
            {
                UpdatesText = Resources.SettingsAboutNoUpdates;
                UpdateAvailable = false;
            }
        }
    }

    public bool IsContainerized => PackageHelper.IsContainerized();

    public string Version =>
        FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

    public bool UpdateChecking
    {
        get => updateChecking;
        set
        {
            updateChecking = value;
            OnPropertyChanged();
        }
    }

    public UpdateCheckResult CheckResult
    {
        get => checkResult;
        set
        {
            checkResult = value;
            OnPropertyChanged();
        }
    }

    public bool IsError
    {
        get => isError;
        set
        {
            isError = value;
            OnPropertyChanged();
        }
    }

    public string ErrorMessage
    {
        get => errorMessage;
        set
        {
            errorMessage = value;
            OnPropertyChanged();
        }
    }

    public string UpdatesText
    {
        get => updatesText;
        set
        {
            updatesText = value;
            OnPropertyChanged();
        }
    }

    public IAsyncRelayCommand CheckUpdatesCommand { get; }
    public RelayCommand OpenStoreCommand { get; }

    public bool UpdateAvailable
    {
        get => updateAvailable;
        set
        {
            updateAvailable = value;
            OnPropertyChanged();
        }
    }

    public float ProgressFloat
    {
        get => progress;
        set
        {
            progress = value;
            OnPropertyChanged();
        }
    }

    public int ProgressPercent
    {
        get => progressPercent;
        set
        {
            progressPercent = value;
            OnPropertyChanged();
        }
    }

    public bool UpdateInstalling
    {
        get => updateInstalling;
        set
        {
            updateInstalling = value;
            OnPropertyChanged();
        }
    }

    public IAsyncRelayCommand UpdateCommand { get; }

    public IRelayCommand CancelCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    private async Task CheckUpdates()
    {
        UpdateChecking = true;
        IsError = false;

        try
        {
            CheckResult = await UpdateManager.CheckUpdates();
            if (CheckResult.UpdateAvailable)
            {
                UpdatesText = Resources.SettingsAboutUpdateAvailable;
                UpdateAvailable = true;
            }
            else
            {
                UpdatesText = Resources.SettingsAboutNoUpdates;
                UpdateAvailable = false;
            }
        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorMessage = $"{Resources.SettingsAboutErrorMessage}: {ex.Message}";
        }

        UpdateChecking = false;
    }

    private void ReportProgress(float value)
    {
        ProgressFloat = value;
        ProgressPercent = (int)(value * 100);
    }

    private void ShowError(Exception ex)
    {
        ShowError(ex.Message);
    }

    private void ShowError(string message)
    {
        IsError = true;
        ErrorMessage = $"{message}";
        UpdateAvailable = false;
        UpdateInstalling = false;
        UpdatesText = Resources.SettingsAboutErrorMessage;
    }

    private async Task Update()
    {
        var progressIndicator = new Progress<float>(ReportProgress);
        UpdateInstalling = true;
        IsError = false;
        ProgressFloat = 0;
        ProgressPercent = 0;

        cancelTokenSource = new CancellationTokenSource();
        var cancellationToken = cancelTokenSource.Token;

        try
        {
            await UpdateManager.Update(checkResult, progressIndicator, cancellationToken);
        }
        catch (Win32Exception ex)
        {
            if (ex.NativeErrorCode == 1223) // Do not show error when user closed installer
                UpdateInstalling = false;
            else
                ShowError(ex);
        }
        catch (TaskCanceledException)
        {
            UpdateInstalling = false;
        }
        catch (RateLimitExceededException)
        {
            ShowError(Resources.SettingsAboutRateLimitError);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}