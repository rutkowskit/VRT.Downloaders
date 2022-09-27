using VRT.Downloaders.Services.Confirmation;
using VRT.Downloaders.ViewModels;

namespace VRT.Downloaders.Maui
{
    public partial class AppShell : Shell, IConfirmationService
    {
        public AppShell(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
        public async Task<bool> Confirm(string message, string title)
        {
            bool answer = await DisplayAlert(title, message, "Yes", "No");
            return answer;
        }
    }
}