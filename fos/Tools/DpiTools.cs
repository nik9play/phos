using System.Drawing;
using System.Windows.Interop;

namespace fos.Tools;

public static class DpiTools
{
    static DpiTools()
    {
        UpdateDpiFactors();
    }

    public static double DpiFactorX { get; private set; } = 1;
    public static double DpiFactorY { get; private set; } = 1;

    internal static void UpdateDpiFactors()
    {
        using (var source = new HwndSource(new HwndSourceParameters()))
        {
            if (source.CompositionTarget?.TransformToDevice != null)
            {
                DpiFactorX = source.CompositionTarget.TransformToDevice.M11;
                DpiFactorY = source.CompositionTarget.TransformToDevice.M22;
                return;
            }
        }

        DpiFactorX = DpiFactorY = 1;
    }

    public static Point ScalePointWithDpi(this Point point)
    {
        return new Point
        {
            X = (int)(point.X / DpiFactorX),
            Y = (int)(point.Y / DpiFactorY)
        };
    }

    public static Rectangle ScaleRectangleWithDpi(this Rectangle rectangle)
    {
        return new Rectangle
        {
            X = (int)(rectangle.X / DpiFactorX),
            Y = (int)(rectangle.Y / DpiFactorY),
            Width = (int)(rectangle.Width / DpiFactorX),
            Height = (int)(rectangle.Height / DpiFactorY)
        };
    }
}