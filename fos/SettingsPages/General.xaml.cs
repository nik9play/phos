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
}