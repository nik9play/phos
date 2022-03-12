using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace fos
{
    /// <summary>
    /// Логика взаимодействия для HotkeyWindow.xaml
    /// </summary>
    public partial class HotkeyWindow : Window
    {
        public HotkeyWindow()
        {
            HideTimer.Tick += (senderT, eT) => {
                HideMe();
                HideTimer.Stop();
            };

            InitializeComponent();
            SetPosition();
        }

        private readonly DispatcherTimer HideTimer = new DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 3)
        };

        public void SetPosition()
        {
            var currentMonitorInfo = MonitorTools.GetCurrentMonitor();

            double factor = VisualTreeHelper.GetDpi(this).DpiScaleX;

            switch (SettingsController.Store.HotkeyPopupLocation)
            {
                case HotkeyPopupLocationEnum.TopLeft:
                    Top = currentMonitorInfo.WorkingArea.Y/ factor;
                    Left = currentMonitorInfo.WorkingArea.X / factor;
                    break;
                case HotkeyPopupLocationEnum.BottomLeft:
                    Top = (currentMonitorInfo.WorkingArea.Y + currentMonitorInfo.WorkingArea.Height) / factor - Height;
                    Left = currentMonitorInfo.WorkingArea.X / factor;
                    break;
                case HotkeyPopupLocationEnum.TopRight:
                    Top = currentMonitorInfo.WorkingArea.Y / factor;
                    Left = (currentMonitorInfo.WorkingArea.X + currentMonitorInfo.WorkingArea.Width) / factor - Width;
                    break;
                case HotkeyPopupLocationEnum.BottomRight:
                    Top = (currentMonitorInfo.WorkingArea.Y + currentMonitorInfo.WorkingArea.Height) / factor - Height;
                    Left = (currentMonitorInfo.WorkingArea.X + currentMonitorInfo.WorkingArea.Width) / factor - Width;
                    break;
                case HotkeyPopupLocationEnum.BottomCenter:
                    Top = (currentMonitorInfo.WorkingArea.Y + currentMonitorInfo.WorkingArea.Height) / factor - Height;
                    Left = (currentMonitorInfo.WorkingArea.X + currentMonitorInfo.WorkingArea.Width / 2) / factor - Width / 2;
                    break;
                case HotkeyPopupLocationEnum.TopCenter:
                    Top = currentMonitorInfo.WorkingArea.Y / factor;
                    Left = (currentMonitorInfo.WorkingArea.X + currentMonitorInfo.WorkingArea.Width / 2) / factor - Width / 2;
                    break;
            }
        }

        private void HideMe()
        {
            switch (SettingsController.Store.HotkeyPopupLocation)
            {
                case HotkeyPopupLocationEnum.TopLeft:
                case HotkeyPopupLocationEnum.TopRight:
                case HotkeyPopupLocationEnum.TopCenter:
                    (FindResource("HidePopupUp") as Storyboard).Begin(this);
                    break;
                case HotkeyPopupLocationEnum.BottomLeft:
                case HotkeyPopupLocationEnum.BottomRight:
                case HotkeyPopupLocationEnum.BottomCenter:
                    (FindResource("HidePopupDown") as Storyboard).Begin(this);
                    break;
            }
            
        }

        private void ShowMe()
        {
            SetPosition();

            if (Visibility == Visibility.Hidden)
            {
                Visibility = Visibility.Visible;
                switch (SettingsController.Store.HotkeyPopupLocation)
                {
                    case HotkeyPopupLocationEnum.TopLeft:
                    case HotkeyPopupLocationEnum.TopRight:
                    case HotkeyPopupLocationEnum.TopCenter:
                        (FindResource("ShowPopupUp") as Storyboard).Begin(this);
                        break;
                    case HotkeyPopupLocationEnum.BottomLeft:
                    case HotkeyPopupLocationEnum.BottomRight:
                    case HotkeyPopupLocationEnum.BottomCenter:
                        (FindResource("ShowPopupDown") as Storyboard).Begin(this);
                        break;
                }
            }

            HideTimer.Stop();
            HideTimer.Start();
        }

        public void SetValue(uint value)
        {
            ShowMe();
            percentText.Text = value.ToString();
            progressBar.Value = value;
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
