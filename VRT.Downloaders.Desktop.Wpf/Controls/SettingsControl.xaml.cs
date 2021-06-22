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
using VRT.Downloaders.Services.Configs;

namespace VRT.Downloaders.Desktop.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public static readonly DependencyProperty SettingsServiceProperty
           = DependencyProperty.Register(nameof(SettingsService), typeof(bool), typeof(IAppSettingsService));
        
        public SettingsControl()
        {
            InitializeComponent();
        }

        public IAppSettingsService SettingsService
        {
            get => (IAppSettingsService)GetValue(SettingsServiceProperty);
            set => SetValue(SettingsServiceProperty, value);
        }
        //public string OutputDirectory
        //{
        //    get => (IAppSettingsService)GetValue(SettingsServiceProperty);
        //    set => SetValue(SettingsServiceProperty, value);
        //}
    }
}
