using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace fos;

public class SliderTools : DependencyObject
{
    public static bool GetMoveToPointOnDrag(DependencyObject obj)
    {
        return (bool)obj.GetValue(MoveToPointOnDragProperty);
    }

    public static void SetMoveToPointOnDrag(DependencyObject obj, bool value)
    {
        obj.SetValue(MoveToPointOnDragProperty, value);
    }

    public static readonly DependencyProperty MoveToPointOnDragProperty = DependencyProperty.RegisterAttached(
        "MoveToPointOnDrag", typeof(bool), typeof(SliderTools), new PropertyMetadata
        {
            PropertyChangedCallback = (obj, changeEvent) =>
            {
                var slider = (Slider)obj;
                if ((bool)changeEvent.NewValue)
                {
                    slider.MouseMove += (sender, args) =>
                    {
                        if (args.LeftButton == MouseButtonState.Pressed && clickedInSlider)
                        {
                            var thumb = (slider.Template.FindName("PART_Track", slider) as System.Windows.Controls.Primitives.Track).Thumb;
                            thumb.RaiseEvent(new MouseButtonEventArgs(args.MouseDevice, args.Timestamp, MouseButton.Left)
                            {
                                RoutedEvent = UIElement.MouseLeftButtonDownEvent,
                                Source = args.Source
                            });
                        }
                    };

                    slider.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, new RoutedEventHandler((sender, args) =>
                    {
                        clickedInSlider = true;
                    }), true);

                    slider.AddHandler(UIElement.PreviewMouseLeftButtonUpEvent, new RoutedEventHandler((sender, args) =>
                    {
                        clickedInSlider = false;
                    }), true);
                }
            }
        });

    private static bool clickedInSlider;
}