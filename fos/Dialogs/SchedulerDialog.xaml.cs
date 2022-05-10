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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using fos.ViewModels;
using ModernWpf.Controls;

namespace fos.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для SchedulerDialog.xaml
    /// </summary>
    public partial class SchedulerDialog : ContentDialog
    {
        public BrightnessSchedulerSettingsElement BrightnessTask { get; set; }

        public SchedulerDialog()
        {
            InitializeComponent();
        }

        private void AddClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //PageBrightnessSchedulerViewModel.BrightnessTasks.Add(new BrightnessSchedulerSettingsElement()
            //{
            //    AllMonitors = false,
            //    DeviceId = "123",
            //    Brightness = 10,
            //    Time = new TimeSpan(1, 1, 1),
            //    Name = "12333"
            //});
        }
    }
}