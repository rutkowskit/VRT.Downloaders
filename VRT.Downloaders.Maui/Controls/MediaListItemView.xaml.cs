using System.Reactive.Linq;
using System.Windows.Input;
using VRT.Downloaders.Presentation.Extensions;
using VRT.Downloaders.Services.Medias;

namespace VRT.Downloaders.Maui.Controls;

public partial class MediaListItemView : Grid
{
    public static readonly BindableProperty MediaProperty = BindableProperty.Create(
        nameof(Media), typeof(MediaInfo), typeof(MediaListItemView), null);

    public static readonly BindableProperty DetailsVisibleProperty = BindableProperty.Create(
        nameof(DetailsVisible), typeof(bool), typeof(MediaListItemView), false);

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
    public bool DetailsVisible
    {
        get => (bool)GetValue(DetailsVisibleProperty);
        set => SetValue(DetailsVisibleProperty, value);
    }

    public ICommand DownloadMediaCommand
    {
        get => (ICommand)GetValue(DownloadMediaCommandProperty);
        set => SetValue(DownloadMediaCommandProperty, value);
    }

    private void OnExpandChange(object sender, CommunityToolkit.Maui.Core.ExpandedChangedEventArgs e)
    {
        MinimumHeightRequest = 10; // this forces the control to resize itself otherwise it will not change its size when it is within a CollectionView        
    }

    private async void OnCopyToClipboardButtonClick(object sender, EventArgs e)
    {        
        if (BindingContext is MediaInfo mi)
        {
            await Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.Default.SetTextAsync(mi.Url.AbsoluteUri);
            _ = ShowMessage("Copied");            
        }
    }

    private async Task ShowMessage(string message)
    {
        uxLastOptionMessageField.DoOnDispatcher(l => l.Text = message);
        await Task.Delay(TimeSpan.FromMilliseconds(2000));
        uxLastOptionMessageField.DoOnDispatcher(l => l.Text = string.Empty);
    }
}