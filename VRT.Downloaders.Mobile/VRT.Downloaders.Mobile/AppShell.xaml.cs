using ReactiveUI;
using VRT.Downloaders.ViewModels;

namespace VRT.Downloaders.Mobile
{
    public partial class AppShell : Xamarin.Forms.Shell, IActivatableView
    {
        public AppShell(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));

            
            BindingContext = viewModel;
            viewModel.OnActivation();
            this.WhenActivated(_ => viewModel.OnActivation()).Discard();
        }        
    }
}
