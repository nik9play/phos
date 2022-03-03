using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace fos.ViewModels
{
    class PageAboutViewModel : INotifyPropertyChanged
    {
        public string Version => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

        public PageAboutViewModel()
        {
            CheckUpdatesCommand = new AsyncRelayCommand(checkUpdates);
            UpdateCommand = new AsyncRelayCommand(Update);
        }

        static private bool updateChecking = false;
        public bool UpdateChecking
        {
            get { return updateChecking; }
            set { updateChecking = value; OnPropertyChanged(); }
        }

        static private UpdateCheckResult checkResult;
        public UpdateCheckResult CheckResult
        {
            get { return checkResult; }
            set { checkResult = value; OnPropertyChanged(); }
        }

        static private bool isError;
        public bool IsError
        {
            get { return isError; }
            set { isError = value; OnPropertyChanged(); }
        }

        static private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; OnPropertyChanged(); }
        }

        static private string updatesText = Properties.Resources.SettingsAboutNoUpdates;
        public string UpdatesText
        {
            get { return updatesText; }
            set { updatesText = value; OnPropertyChanged(); }
        }

        public IAsyncRelayCommand CheckUpdatesCommand { get; }
        private async Task checkUpdates()
        {
            UpdateChecking = true;
            IsError = false;

            try
            {
                CheckResult = await UpdateManager.CheckUpdates();
                if (CheckResult.UpdateAvailable)
                {
                    UpdatesText = Properties.Resources.SettingsAboutUpdateAvailable;
                    UpdateAvailable = true;
                }
                else
                {
                    UpdatesText = Properties.Resources.SettingsAboutNoUpdates;
                    UpdateAvailable = false;
                }
            }
            catch (Exception ex)
            {
                IsError = true;
                ErrorMessage = $"{Properties.Resources.SettingsAboutErrorMessage}: {ex.Message}";
            }

            UpdateChecking = false;
        }

        static private bool updateAvailable;
        public bool UpdateAvailable
        {
            get { return updateAvailable; }
            set { updateAvailable = value; OnPropertyChanged(); }
        }

        static private float progress = 0;
        public float ProgressFloat
        {
            get { return progress; }
            set { progress = value; OnPropertyChanged(); }
        }

        static private int progressPercent = 0;
        public int ProgressPercent
        {
            get { return progressPercent; }
            set { progressPercent = value; OnPropertyChanged(); }
        }

        static private bool updateInstalling;
        public bool UpdateInstalling
        {
            get { return updateInstalling; }
            set { updateInstalling = value; OnPropertyChanged(); }
        }

        private void ReportProgress(float value)
        {
            ProgressFloat = value;
            ProgressPercent = (int)(value * 100);
        }

        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private void ShowError(Exception ex)
        {
            IsError = true;
            ErrorMessage = $"{ex.Message}";
            UpdateAvailable = false;
            UpdateInstalling = false;
            UpdatesText = Properties.Resources.SettingsAboutErrorMessage;
        }

        public IAsyncRelayCommand UpdateCommand { get; }
        private async Task Update()
        {
            var progressIndicator = new Progress<float>(ReportProgress);
            UpdateInstalling = true;
            IsError = false;
            ProgressFloat = 0;
            ProgressPercent = 0;

            CancellationToken cancellationToken = cancelTokenSource.Token;

            try
            {
                await UpdateManager.Update(checkResult, progressIndicator, cancellationToken);
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 1223)
                    UpdateInstalling = false;
                else
                    ShowError(ex);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
