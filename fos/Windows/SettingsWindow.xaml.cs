using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using fos.SettingsPages;
using fos.Win32Interops;
using fos.Workarounds;
using ModernWpf;
using ModernWpf.Controls;

namespace fos;

/// <summary>
///     Логика взаимодействия для SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : Window
{
    private bool _isActive;

    public SettingsWindow()
    {
        InitializeComponent();
        _hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
    }

    private readonly IntPtr _hWnd;

    private void SetMica()
    {
        if (Environment.OSVersion.IsAtLeast(OSVersions.WIN11_INSIDER))
        {
            int immersiveDarkMode = 0x00;
            
            if (ThemeTools.CurrentTheme == ApplicationTheme.Dark)
                immersiveDarkMode = 0x01;

            DwmAPI.DwmSetWindowAttribute(_hWnd, DwmAPI.DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE,
                ref immersiveDarkMode, Marshal.SizeOf(typeof(int)));

            int backdropValue = 0x02;

            DwmAPI.DwmSetWindowAttribute(_hWnd, DwmAPI.DwmWindowAttribute.DWMWA_SYSTEMBACKDROP_TYPE,
                ref backdropValue, Marshal.SizeOf(typeof(int)));
        }
    }
    
    private void NavigationView_SelectionChanged(NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        var selectedItem = (NavigationViewItem)args.SelectedItem;
        if (selectedItem != null)
        {
            var Tag = (string)selectedItem.Tag;

            switch (Tag)
            {
                case "General":
                    ContentFrame.Navigate(typeof(General));
                    NavView.Header = Properties.Resources.SettingsGeneral;
                    break;
                case "Hotkeys":
                    ContentFrame.Navigate(typeof(Hotkeys));
                    NavView.Header = Properties.Resources.SettingsHotkeys;
                    break;
                case "About":
                    ContentFrame.Navigate(typeof(About));
                    NavView.Header = Properties.Resources.SettingsAbout;
                    break;
                case "Monitors":
                    ContentFrame.Navigate(typeof(SettingsPages.Monitors));
                    NavView.Header = Properties.Resources.SettingsMonitors;
                    break;
                case "BrightnessScheduler":
                    ContentFrame.Navigate(typeof(SettingsPages.BrightnessScheduler));
                    NavView.Header = "sc";
                    break;
            }
        }
    }

    public void OpenAboutPage()
    {
        NavView.SelectedItem = AboutItem;
    }

    protected override void OnActivated(EventArgs e)
    {
        if (!_isActive)
        {
            RenderLoopFix.ApplyFix();
            _isActive = true;
        }

        base.OnActivated(e);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        SettingsController.SaveSettings();
    }

    private void NavView_Unloaded(object sender, RoutedEventArgs e)
    {
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
    }

    private void SettingsWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        //SetMica();
    }
}