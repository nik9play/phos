namespace fos.SettingsPages
{
    /// <summary>
    /// Логика взаимодействия для Monitors.xaml
    /// </summary>
    public partial class Monitors : ModernWpf.Controls.Page
    {
        public Monitors()
        {
            InitializeComponent();
            DataContext = new ViewModels.PageMonitorsViewModel();
        }
    }
}
