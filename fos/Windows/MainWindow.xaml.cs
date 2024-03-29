﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using fos.Monitors;
using fos.Tools;
using fos.ViewModels;
using ModernWpf;

namespace fos;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Visibility = Visibility.Hidden;
        DataContext = new MainWindowViewModel();
        ThemeTools.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(object sender, ThemeChangingArgs e)
    {
        SetColors();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        SetColors();
    }

    public void SetColors()
    {
        if (ThemeTools.CurrentTheme == ApplicationTheme.Dark)
        {
            Resources["MainPanelBackground"] = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
            Resources["BottomPanelBackground"] = new SolidColorBrush(Color.FromRgb(0x1c, 0x1c, 0x1c));
            Resources["SeparatorBrush"] = new SolidColorBrush(Color.FromRgb(0x25, 0x25, 0x25));
        }
        else
        {
            Resources["MainPanelBackground"] = new SolidColorBrush(Color.FromRgb(0xf2, 0xf2, 0xf2));
            Resources["BottomPanelBackground"] = new SolidColorBrush(Color.FromRgb(0xee, 0xee, 0xee));
            Resources["SeparatorBrush"] = new SolidColorBrush(Color.FromRgb(0xe0, 0xe0, 0xe0));
        }
    }

    public void SetPosition()
    {
        var currentMonitorInfo = MonitorTools.GetCurrentMonitor();

        var factor = DpiTools.DpiFactorX;

        if (currentMonitorInfo.WorkingArea.Height < currentMonitorInfo.Bounds.Height)
        {
            if (currentMonitorInfo.WorkingArea.Y > 0)
            {
                Left = currentMonitorInfo.WorkingArea.Right / factor - ActualWidth;
                Top = currentMonitorInfo.WorkingArea.Top / factor + 5;
                ContentGrid.VerticalAlignment = VerticalAlignment.Top;
            }
            else
            {
                Left = currentMonitorInfo.WorkingArea.Right / factor - ActualWidth;
                Top = currentMonitorInfo.WorkingArea.Bottom / factor - ActualHeight;
                ContentGrid.VerticalAlignment = VerticalAlignment.Bottom;
            }
        }
        else if (currentMonitorInfo.WorkingArea.Width < currentMonitorInfo.Bounds.Width)
        {
            if (currentMonitorInfo.WorkingArea.X > 0)
            {
                Left = currentMonitorInfo.WorkingArea.Left / factor;
                Top = currentMonitorInfo.WorkingArea.Bottom / factor - ActualHeight;
                ContentGrid.VerticalAlignment = VerticalAlignment.Bottom;
            }
            else
            {
                Left = currentMonitorInfo.WorkingArea.Right / factor - ActualWidth;
                Top = currentMonitorInfo.WorkingArea.Bottom / factor - ActualHeight;
                ContentGrid.VerticalAlignment = VerticalAlignment.Bottom;
            }
        }

        Height = currentMonitorInfo.WorkingArea.Height / factor;

        //Left = desktopWorkingArea.Right / factor - ActualWidth;
        //Top = desktopWorkingArea.Bottom / factor - ActualHeight;
    }

    public void TogglePopup()
    {
        SetPosition();

        if (Visibility != Visibility.Hidden) return;

        Visibility = Visibility.Visible;

        if (ThemeTools.IsAnimationsEnabled)
            (TryFindResource("ShowPopup") as Storyboard)?.Begin(this);
        else
            (TryFindResource("ShowPopupNoAnim") as Storyboard)?.Begin(this);

        Activate();
    }

    public void PreloadWindow()
    {
        Visibility = Visibility.Visible;
        Visibility = Visibility.Hidden;
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
        if (ThemeTools.IsAnimationsEnabled)
            (TryFindResource("HidePopup") as Storyboard)?.Begin(this);
        else
            (TryFindResource("HidePopupNoAnim") as Storyboard)?.Begin(this);
    }

    private void AppBarButton_Click(object sender, RoutedEventArgs e)
    {
        WindowManager.OpenSettingsWindow();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetPosition();
    }

    private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.System && e.SystemKey == Key.F4) e.Handled = true;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        
    }
}