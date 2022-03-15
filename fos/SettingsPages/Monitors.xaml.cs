using fos.ViewModels;
using ModernWpf.Controls;

namespace fos.SettingsPages
{
    /// <summary>
    ///     Логика взаимодействия для Monitors.xaml
    /// </summary>
    public partial class Monitors : Page
    {
        public Monitors()
        {
            InitializeComponent();
            DataContext = new PageMonitorsViewModel();
        }
    }
}