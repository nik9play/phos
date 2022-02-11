﻿using System;
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
            var desktopWorkingArea = System.Windows.Forms.Screen.GetWorkingArea(System.Windows.Forms.Control.MousePosition);
            var desktopBounds = System.Windows.Forms.Screen.GetBounds(System.Windows.Forms.Control.MousePosition);

            double factor = VisualTreeHelper.GetDpi(this).DpiScaleX;

            switch (SettingsController.Store.HotkeyPopupLocation)
            {
                case HotkeyPopupLocationEnum.TopLeft:
                    Top = desktopWorkingArea.Y / factor;
                    Left = desktopWorkingArea.X / factor;
                    break;
                case HotkeyPopupLocationEnum.BottomLeft:
                    Top = (desktopWorkingArea.Y + desktopWorkingArea.Height) / factor - Height;
                    Left = desktopWorkingArea.X / factor;
                    break;
                case HotkeyPopupLocationEnum.TopRight:
                    Top = desktopWorkingArea.Y / factor;
                    Left = (desktopWorkingArea.X + desktopWorkingArea.Width) / factor - Width;
                    break;
                case HotkeyPopupLocationEnum.BottomRight:
                    Top = (desktopWorkingArea.Y + desktopWorkingArea.Height) / factor - Height;
                    Left = (desktopWorkingArea.X + desktopWorkingArea.Width) / factor - Width;
                    break;
                case HotkeyPopupLocationEnum.BottomCenter:
                    Top = (desktopWorkingArea.Y + desktopWorkingArea.Height) / factor - Height;
                    Left = (desktopWorkingArea.X + desktopWorkingArea.Width / 2) / factor - Width / 2;
                    break;
                case HotkeyPopupLocationEnum.TopCenter:
                    Top = desktopWorkingArea.Y / factor;
                    Left = (desktopWorkingArea.X + desktopWorkingArea.Width / 2) / factor - Width / 2;
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
    }
}
