using VRT.Downloaders.ViewModels;

namespace VRT.Downloaders.Maui.Pages;

public partial class MainPage : ContentPage
{        
    public MainPage(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }    
}