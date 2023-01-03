using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Maui.Pages;
using VRT.Downloaders.Presentation.ViewModels;

namespace VRT.Downloaders.Maui;
public partial class AppShell : Shell, IConfirmationService
{
    public AppShell(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        RegisterRouting();
        BindingContext = viewModel;
    }

    private void RegisterRouting()
    {
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(ErrorPage), typeof(ErrorPage));
    }

    public async Task<bool> Confirm(string message, string title)
    {
        bool answer = await DisplayAlert(title, message, "Yes", "No");
        return answer;
    }
}