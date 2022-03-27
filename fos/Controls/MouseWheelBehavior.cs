using System.Windows;
using System.Windows.Controls;

namespace fos;

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
            var slider = (Slider)d;
            slider.PreviewMouseWheel += (s, arg) =>
            {
                var sl = s as Slider;

                if (arg.Delta < 0)
                {
                    var newVal = sl.Value - sl.SmallChange * GetValue(sl);
                    sl.Value = newVal < sl.Minimum ? sl.Minimum : newVal;
                }
                else
                {
                    var newVal = sl.Value + sl.SmallChange * GetValue(sl);
                    sl.Value = newVal > sl.Maximum ? sl.Maximum : newVal;
                }
            };
        }
    }
}