
namespace VRT.Downloaders.Maui.Pages;

public partial class ErrorPage : ContentPage, IQueryAttributable
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

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ErrorMessage = query.TryGetValue(nameof(ErrorMessage), out var errMessage) && errMessage is string errMessageText
            ? errMessageText
            : string.Empty;
    }
}