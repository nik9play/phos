﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace fos
{
    public class SliderTools : DependencyObject
    {
        public static readonly DependencyProperty MoveToPointOnDragProperty = DependencyProperty.RegisterAttached(
            "MoveToPointOnDrag", typeof(bool), typeof(SliderTools), new PropertyMetadata
            {
                PropertyChangedCallback = (obj, changeEvent) =>
                {
                    var slider = (Slider)obj;
                    if ((bool)changeEvent.NewValue)
                        slider.MouseMove += (obj2, mouseEvent) =>
                        {
                            if (mouseEvent.LeftButton == MouseButtonState.Pressed)
                                slider.RaiseEvent(new MouseButtonEventArgs(mouseEvent.MouseDevice, mouseEvent.Timestamp,
                                    MouseButton.Left)
                                {
                                    RoutedEvent = UIElement.PreviewMouseLeftButtonDownEvent,
                                    Source = mouseEvent.Source
                                });
                        };
                }
            });

        public static bool GetMoveToPointOnDrag(DependencyObject obj)
        {
            return (bool)obj.GetValue(MoveToPointOnDragProperty);
        }

        public static void SetMoveToPointOnDrag(DependencyObject obj, bool value)
        {
            obj.SetValue(MoveToPointOnDragProperty, value);
        }
    }
}