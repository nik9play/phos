using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace fos;

/// <summary>
///     Логика взаимодействия для WelcomeWindow.xaml
/// </summary>
public partial class WelcomeWindow : Window
{
    private readonly DispatcherTimer _closingTimer = new()
    {
        Interval = TimeSpan.FromMilliseconds(400)
    };

    private bool _isClosing;

    public WelcomeWindow()
    {
        InitializeComponent();

        _closingTimer.Tick += (s, e) =>
        {
            Close();
            _closingTimer.Stop();
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
            DragMove();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (_isClosing) return;
        SettingsController.Store.FirstStart = false;
        SettingsController.SaveSettings();

        (TryFindResource("ClosingAnimation") as Storyboard)?.Begin(this);

        e.Cancel = true;
        _isClosing = true;
        _closingTimer.Start();
    }
}