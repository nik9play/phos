using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using fos.Win32Interops;

namespace fos.Tools;

public class PackageHelper
{
    private const long APPMODEL_ERROR_NO_PACKAGE = 15700L;

    private static bool? hasIdentity;

    public static bool HasIdentity()
    {
        if (hasIdentity == null)
        {
            int length = 0;
            var sb = new StringBuilder(0);
            Kernel32.GetCurrentPackageFullName(ref length, sb);

            sb = new StringBuilder(length);
            int error = Kernel32.GetCurrentPackageFullName(ref length, sb);

            hasIdentity = error != APPMODEL_ERROR_NO_PACKAGE;
        }

        return hasIdentity.Value;
    }

    private static bool? isContainerized;

    public static bool IsContainerized()
    {
        if (isContainerized == null)
        {
            // If MSIX or sparse
            if (HasIdentity())
            {
                // Sparse is identified if EXE is running outside of installed package location
                var packageInstalledLocation = Package.Current.InstalledLocation.Path;
                var actualExeFullPath = Process.GetCurrentProcess().MainModule.FileName;

                // If inside package location
                if (actualExeFullPath.StartsWith(packageInstalledLocation))
                {
                    isContainerized = true;
                }
                else
                {
                    isContainerized = false;
                }
            }

            // Plain Win32
            else
            {
                isContainerized = false;
            }
        }

        return isContainerized.Value;
    }

    public static async Task<StartupTaskState> GetStartupTaskState()
    {
        StartupTask startupTask = await StartupTask.GetAsync("phosStartupTask");
        Debug.WriteLine(startupTask.State);

        return startupTask.State;
    }

    public static async void SetStartupTaskState(bool state)
    {
        StartupTask startupTask = await StartupTask.GetAsync("phosStartupTask");

        if (state)
            await startupTask.RequestEnableAsync();
        else
            startupTask.Disable();
    }
}