using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace fos.Tools
{
    public static class DpiTools
    {
        public static double DpiFactorX { get; private set; } = 1;
        public static double DpiFactorY { get; private set; } = 1;

        static DpiTools()
        {
            UpdateDpiFactors();
        }

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

        public static System.Drawing.Point ScalePointWithDpi(this System.Drawing.Point point)
        {
            return new System.Drawing.Point
            {
                X = (int)(point.X / DpiFactorX),
                Y = (int)(point.Y / DpiFactorY),
            };
        }

        public static System.Drawing.Rectangle ScaleRectangleWithDpi(this System.Drawing.Rectangle rectangle)
        {
            return new System.Drawing.Rectangle
            {
                X = (int)(rectangle.X / DpiFactorX),
                Y = (int)(rectangle.Y / DpiFactorY),
                Width = (int)(rectangle.Width / DpiFactorX),
                Height = (int)(rectangle.Height / DpiFactorY)
            };
        }
    }
}
