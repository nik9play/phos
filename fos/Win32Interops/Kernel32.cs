using System;
using System.Runtime.InteropServices;
using System.Text;

namespace fos.Win32Interops;

public class Kernel32
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int RegisterApplicationRestart([MarshalAs(UnmanagedType.LPWStr)] string pwzCommandLine, RestartFlags dwFlags);

    [Flags]
    internal enum RestartFlags
    {
        /// <summary>None of the options below.</summary>
        NONE = 0,

        /// <summary>Do not restart the process if it terminates due to an unhandled exception.</summary>
        RESTART_NO_CRASH = 1,

        /// <summary>Do not restart the process if it terminates due to the application not responding.</summary>
        RESTART_NO_HANG = 2,

        /// <summary>Do not restart the process if it terminates due to the installation of an update.</summary>
        RESTART_NO_PATCH = 4,

        /// <summary>Do not restart the process if the computer is restarted as the result of an update.</summary>
        RESTART_NO_REBOOT = 8
    }
}