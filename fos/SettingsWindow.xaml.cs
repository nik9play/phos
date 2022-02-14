using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace fos
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (ModernWpf.Controls.NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string Tag = (string)selectedItem.Tag;

                switch (Tag)
                {
                    case "General":
                        ContentFrame.Navigate(typeof(SettingsPages.General));
                        NavView.Header = Properties.Resources.SettingsGeneral;
                        break;
                    case "Hotkeys":
                        ContentFrame.Navigate(typeof(SettingsPages.Hotkeys));
                        NavView.Header = Properties.Resources.SettingsHotkeys;
                        break;
                    case "About":
                        ContentFrame.Navigate(typeof(SettingsPages.About));
                        NavView.Header = Properties.Resources.SettingsAbout;
                        break;
                }
            }
        }

        private bool _isActive;

        protected override void OnActivated(EventArgs e)
        {
            if (!_isActive)
            {
                Workarounds.RenderLoopFix.ApplyFix();
                _isActive = true;
            }

            base.OnActivated(e);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SettingsController.SaveSettings();
        }
    }
}
