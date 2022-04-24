using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using fos.Monitors;
using fos.Tools;

namespace fos;

/// <summary>
///     Логика взаимодействия для HotkeyWindow.xaml
/// </summary>
public partial class HotkeyWindow : Window
{
    private readonly DispatcherTimer HideTimer = new()
    {
        Interval = new TimeSpan(0, 0, 3)
    };

    public HotkeyWindow()
    {
        HideTimer.Tick += (senderT, eT) =>
        {
            HideMe();
            HideTimer.Stop();
        };

        InitializeComponent();
        SetPosition();
    }

    public void SetPosition()
    {
        var currentMonitorInfo = MonitorTools.GetCurrentMonitor();

        var factor = DpiTools.DpiFactorX;

        switch (SettingsController.Store.HotkeyPopupLocation)
        {
            case HotkeyPopupLocationEnum.TopLeft:
                Top = currentMonitorInfo.WorkingArea.Y / factor;
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
                Left = (currentMonitorInfo.WorkingArea.X + currentMonitorInfo.WorkingArea.Width / 2) / factor -
                       Width / 2;
                break;
            case HotkeyPopupLocationEnum.TopCenter:
                Top = currentMonitorInfo.WorkingArea.Y / factor;
                Left = (currentMonitorInfo.WorkingArea.X + currentMonitorInfo.WorkingArea.Width / 2) / factor -
                       Width / 2;
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
                if (!ThemeTools.IsAnimationsEnabled)
                    (TryFindResource("HidePopupUpNoAnim") as Storyboard)?.Begin(this);
                else
                    (TryFindResource("HidePopupUp") as Storyboard)?.Begin(this);
                break;
            case HotkeyPopupLocationEnum.BottomLeft:
            case HotkeyPopupLocationEnum.BottomRight:
            case HotkeyPopupLocationEnum.BottomCenter:
                if (!ThemeTools.IsAnimationsEnabled)
                    (TryFindResource("HidePopupDownNoAnim") as Storyboard)?.Begin(this);
                else
                    (TryFindResource("HidePopupDown") as Storyboard)?.Begin(this);
                break;
        }
    }

    private void ShowMe()
    {
        SetPosition();

        HideTimer.Stop();
        HideTimer.Start();

        if (Visibility == Visibility.Hidden)
        {
            Visibility = Visibility.Visible;

            switch (SettingsController.Store.HotkeyPopupLocation)
            {
                case HotkeyPopupLocationEnum.TopLeft:
                case HotkeyPopupLocationEnum.TopRight:
                case HotkeyPopupLocationEnum.TopCenter:
                    if (!ThemeTools.IsAnimationsEnabled)
                        (TryFindResource("ShowPopupUpNoAnim") as Storyboard)?.Begin(this);
                    else
                        (TryFindResource("ShowPopupUp") as Storyboard)?.Begin(this);
                    break;
                case HotkeyPopupLocationEnum.BottomLeft:
                case HotkeyPopupLocationEnum.BottomRight:
                case HotkeyPopupLocationEnum.BottomCenter:
                    if (!ThemeTools.IsAnimationsEnabled)
                        (TryFindResource("ShowPopupDownNoAnim") as Storyboard)?.Begin(this);
                    else
                        (TryFindResource("ShowPopupDown") as Storyboard)?.Begin(this);
                    break;
            }
        }
    }

    public void SetValue(uint value)
    {
        ShowMe();
        percentText.Text = value.ToString();
        progressBar.Value = value;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.System && e.SystemKey == Key.F4) e.Handled = true;
    }
}