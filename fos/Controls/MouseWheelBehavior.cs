using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace fos
{
    public class MouseWheelBehavior
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
                "Value", typeof(int), typeof(MouseWheelBehavior), new PropertyMetadata(Value));

        public static void SetValue(Slider element, int value)
        {
            element.SetValue(ValueProperty, value);
        }

        public static int GetValue(Slider element)
        {
            return (int)element.GetValue(ValueProperty);
        }

        private static void Value(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Slider)
            {
                Slider slider = (Slider)d;
                slider.PreviewMouseWheel += (s, arg) =>
                {
                    Slider sl = s as Slider;
                    
                    if (arg.Delta < 0)
                    {
                        double newVal = sl.Value - sl.SmallChange * GetValue(sl);
                        if (newVal >= sl.Minimum)
                        {
                            sl.Value = newVal;
                        }
                    }
                    else
                    {
                        double newVal = sl.Value + sl.SmallChange * GetValue(sl);
                        if (newVal <= sl.Maximum)
                        {
                            sl.Value = newVal;
                        }
                    }
                };
            }
        }
    }
}
