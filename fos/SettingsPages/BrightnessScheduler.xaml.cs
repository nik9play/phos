using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel;
using fos.Tools;
using fos.ViewModels;
using Microsoft.Toolkit.Uwp.Notifications;

namespace fos.SettingsPages;

public partial class BrightnessScheduler : ModernWpf.Controls.Page
{
    public BrightnessScheduler()
    {
        InitializeComponent();
        DataContext = new PageBrightnessSchedulerViewModel();
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        new ToastContentBuilder()
                .AddArgument("action", "test")
                .AddText("dfasdasdf")
                .Show();
    }
}
