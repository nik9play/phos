using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using fos.ViewModels;

namespace fos
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void SetPosition()
        {
            var currentMonitorInfo = MonitorTools.GetCurrentMonitor();

            var factor = VisualTreeHelper.GetDpi(this).DpiScaleX;

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
            var storyBoard = TryFindResource("ShowPopup") as Storyboard;
            storyBoard?.Begin(this);
            Activate();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            var storyBoard = TryFindResource("HidePopup") as Storyboard;
            storyBoard?.Begin(this);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.OpenSettingsWindow();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPosition();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4) e.Handled = true;
        }
    }
}