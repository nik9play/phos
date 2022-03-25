using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using fos.SettingsPages;
using fos.Workarounds;
using ModernWpf.Controls;

namespace fos
{
    /// <summary>
    ///     Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private bool _isActive;

        public SettingsWindow()
        {
            InitializeComponent();
            Icon = BitmapFrame.Create(new Uri("pack://application:,,,/Assets/TaskBarIcon.ico",
                UriKind.RelativeOrAbsolute));
            var factor = VisualTreeHelper.GetDpi(this).DpiScaleX;

            if (factor > 1.0)
                Icon = BitmapFrame.Create(new Uri("pack://application:,,,/Assets/TaskBarIcon2.ico",
                    UriKind.RelativeOrAbsolute));
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
                        ContentFrame.Navigate(typeof(Monitors));
                        NavView.Header = Properties.Resources.SettingsMonitors;
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
    }
}