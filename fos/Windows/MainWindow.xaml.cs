using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModernWpf;

namespace fos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ViewModels.MainWindowViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void SetPosition()
        {
            var currentMonitorInfo = MonitorTools.GetCurrentMonitor();

            double factor = VisualTreeHelper.GetDpi(this).DpiScaleX;

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
            (FindResource("ShowPopup") as Storyboard).Begin(this);
            Activate();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            (FindResource("HidePopup") as Storyboard).Begin(this);
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
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }
    }
}
