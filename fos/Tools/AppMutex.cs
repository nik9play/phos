using System.Threading;

namespace fos.Tools;

public static class AppMutex
{
    public static Mutex CurrentMutex { get; set; }

    public static void RealeseMutex()
    {
        CurrentMutex.ReleaseMutex();
        CurrentMutex.Dispose();
    }
}