namespace VRT.Downloaders.Maui.Pages;

[QueryProperty(nameof(ErrorMessage), "ErrorMessage")]
public partial class ErrorPage : ContentPage
{
    public ErrorPage()
    {
        InitializeComponent();
    }
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
    }
    public string? ErrorMessage { get; set; }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {           
        uxErrorField.Text = ErrorMessage;
    }    
}