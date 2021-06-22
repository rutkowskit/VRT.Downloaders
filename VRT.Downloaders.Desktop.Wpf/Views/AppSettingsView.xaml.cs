using System.Windows;
using VRT.Downloaders.ViewModels;

namespace VRT.Downloaders.Desktop.Wpf.Views
{
    /// <summary>
    /// Interaction logic for AppSettingsView.xaml
    /// </summary>
    public partial class AppSettingsView : Window
    {
        public AppSettingsView(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
