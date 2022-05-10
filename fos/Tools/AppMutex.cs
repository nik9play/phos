using System.Threading;

namespace fos.Tools;

public static class AppMutex
{
    public static Mutex CurrentMutex { get; set; }

    public static void UnrealeseMutex()
    {
        CurrentMutex.ReleaseMutex();
        CurrentMutex.Dispose();
    }
}