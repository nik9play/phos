using Microsoft.Win32;
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
    /// Логика взаимодействия для WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();

            closingTimer.Tick += (s, e) =>
            {
                Close();
                closingTimer.Stop();
            };
        }

        private void video_MediaEnded(object sender, RoutedEventArgs e)
        {
            video.Position = new TimeSpan(0, 0, 0);
            //video.Play();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool IsClosing = false;
        private DispatcherTimer closingTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(400)
        };

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsClosing)
            {
                SettingsController.Store.FirstStart = false;
                SettingsController.SaveSettings();

                (FindResource("ClosingAnimation") as Storyboard).Begin(this);

                e.Cancel = true;
                IsClosing = true;
                closingTimer.Start();
            }
        }
    }
}
