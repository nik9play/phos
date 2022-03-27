using fos.ViewModels;
using ModernWpf.Controls;

namespace fos.SettingsPages;

/// <summary>
///     Логика взаимодействия для About.xaml
/// </summary>
public partial class About : Page
{
    public About()
    {
        InitializeComponent();
        DataContext = new PageAboutViewModel();
    }
}