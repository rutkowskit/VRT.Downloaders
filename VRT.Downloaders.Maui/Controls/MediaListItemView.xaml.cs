namespace VRT.Downloaders.Maui.Controls;

[DependencyProperty<string>("OutputFileName", DefaultUpdateSourceTrigger = SourceTrigger.PropertyChanged)]
[DependencyProperty<bool>("DetailsVisible", IsReadOnly = true)]
[DependencyProperty<string>("MediaFormatDescription", IsReadOnly = true)]
[DependencyProperty<string>("MediaTitle", IsReadOnly = true)]
public partial class MediaListItemView : Grid
{
    // Do not remove the explicit property definitions, they are required for Binding when SourceGen is used.
    public static readonly BindableProperty MediaProperty = BindableProperty.Create(
        nameof(Media), typeof(MediaInfo), typeof(MediaListItemView), null, propertyChanged: OnMediaChanged);

    public static readonly BindableProperty DownloadMediaCommandProperty = BindableProperty.Create(
        nameof(DownloadMediaCommand), typeof(ICommand), typeof(MediaListItemView), null);

    public MediaListItemView()
    {
        InitializeComponent();
    }

    public MediaInfo Media
    {
        get => (MediaInfo)GetValue(MediaProperty);
        set => SetValue(MediaProperty, value);
    }
    
    public ICommand DownloadMediaCommand
    {
        get => (ICommand)GetValue(DownloadMediaCommandProperty);
        set => SetValue(DownloadMediaCommandProperty, value);
    }

    private void OnExpandChange(object? sender, CommunityToolkit.Maui.Core.ExpandedChangedEventArgs e)
    {
        MinimumHeightRequest = 10; // this forces the control to resize itself otherwise it will not change its size when it is within a CollectionView        
    }

    private async void OnCopyToClipboardButtonClick(object? sender, EventArgs e)
    {
        if (BindingContext is MediaInfo mi)
        {
            await Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.Default.SetTextAsync(mi.Url.AbsoluteUri);
            _ = ShowMessage("Copied");
        }
    }

    private async Task ShowMessage(string message)
    {
        await uxLastOptionMessageField.DoOnDispatcher(l => l.Text = message);
        await Task.Delay(TimeSpan.FromMilliseconds(2000));
        await uxLastOptionMessageField.DoOnDispatcher(l => l.Text = string.Empty);
    }

    partial void OnOutputFileNameChanged(string? newValue)
    {
        Media?.OutputFileName = newValue;
    }
    static void OnMediaChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if(bindable is not MediaListItemView listItem)
        {
            return;
        }
        MediaInfo v = newValue is MediaInfo m ? m : MediaInfo.Empty;
        listItem.MediaTitle = v.Title;
        listItem.MediaFormatDescription = v.FormatDescription;
        listItem.OutputFileName = v?.OutputFileName;
    }
}