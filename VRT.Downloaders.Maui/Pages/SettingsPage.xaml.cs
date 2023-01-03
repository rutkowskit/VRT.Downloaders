using VRT.Downloaders.Presentation.ViewModels;

namespace VRT.Downloaders.Maui.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();        
        BindingContext = viewModel;
    }
}