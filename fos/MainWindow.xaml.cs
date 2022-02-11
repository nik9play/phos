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
            UpdateTheme(ThemeTools.CurrentTheme, ThemeTools.CurrentAccentColor);
            ThemeTools.ThemeChanged += ThemeTools_ThemeChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //(FindResource("HidePopup") as Storyboard).Begin(this);
            
        }

        private void ThemeTools_ThemeChanged(object sender, ThemeChangingArgs e)
        {
            UpdateTheme(e.CurrentTheme, e.CurrentAccentColor);
        }

        private void UpdateTheme(ApplicationTheme CurrentTheme, Color CurrentAccentColor)
        {
            ThemeManager.Current.ApplicationTheme = CurrentTheme;
            ThemeManager.Current.AccentColor = CurrentAccentColor;

            if (CurrentTheme == ApplicationTheme.Light)
                trayIcon.Icon = new System.Drawing.Icon(Properties.Resources.iconBlack, System.Windows.Forms.SystemInformation.SmallIconSize);
            else
                trayIcon.Icon = new System.Drawing.Icon(Properties.Resources.iconWhite, System.Windows.Forms.SystemInformation.SmallIconSize);
        }

        public void SetPosition()
        {
            var desktopWorkingArea = System.Windows.Forms.Screen.GetWorkingArea(System.Windows.Forms.Control.MousePosition);
            var desktopBounds = System.Windows.Forms.Screen.GetBounds(System.Windows.Forms.Control.MousePosition);

            double factor = VisualTreeHelper.GetDpi(this).DpiScaleX;

            if (desktopWorkingArea.Height < desktopBounds.Height)
            {
                if (desktopWorkingArea.Y > 0)
                {
                    Left = desktopWorkingArea.Right / factor - ActualWidth;
                    Top = desktopWorkingArea.Top / factor + 5;
                    ContentGrid.VerticalAlignment = VerticalAlignment.Top;
                }
                else
                {
                    Left = desktopWorkingArea.Right / factor - ActualWidth;
                    Top = desktopWorkingArea.Bottom / factor - ActualHeight;
                    ContentGrid.VerticalAlignment = VerticalAlignment.Bottom;
                }
            }
            else if (desktopWorkingArea.Width < desktopBounds.Width)
            {
                if (desktopWorkingArea.X > 0)
                {
                    Left = desktopWorkingArea.Left / factor;
                    Top = desktopWorkingArea.Bottom / factor - ActualHeight;
                    ContentGrid.VerticalAlignment = VerticalAlignment.Bottom;
                }
                else
                {
                    Left = desktopWorkingArea.Right / factor - ActualWidth;
                    Top = desktopWorkingArea.Bottom / factor - ActualHeight;
                    ContentGrid.VerticalAlignment = VerticalAlignment.Bottom;
                }
            }
            
            Height = desktopWorkingArea.Height / factor;

            //Left = desktopWorkingArea.Right / factor - ActualWidth;
            //Top = desktopWorkingArea.Bottom / factor - ActualHeight;
        }

        private void trayIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            SetPosition();

            if (Visibility == Visibility.Hidden)
            {
                Visibility = Visibility.Visible;
                (FindResource("ShowPopup") as Storyboard).Begin(this);
                Activate();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            (FindResource("HidePopup") as Storyboard).Begin(this);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //SettingsController.OpenSettingsFile();
            //WindowManager.hotkeyWindow.SetPosition();
            var win = new SettingsWindow();
            win.Show();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPosition();
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
