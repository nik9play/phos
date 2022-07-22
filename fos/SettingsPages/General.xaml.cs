using System.Diagnostics;
using System.Windows;
using fos.ViewModels;
using ModernWpf.Controls;

namespace fos.SettingsPages;

/// <summary>
///     Логика взаимодействия для General.xaml
/// </summary>
public partial class General : Page
{
    public General()
    {
        InitializeComponent();
        DataContext = new PageGeneralViewModel();
    }

    private void OpenSettingsApp(object sender, RoutedEventArgs e)
    {
        var process = new Process();
        process.StartInfo.FileName = "ms-settings:startupapps";
        process.StartInfo.UseShellExecute = true;
        process.Start();
    }
}