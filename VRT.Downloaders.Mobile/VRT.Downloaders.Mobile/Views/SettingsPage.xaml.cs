using VRT.Downloaders.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VRT.Downloaders.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = App.Services.GetService<SettingsViewModel>();
        }
    }
}