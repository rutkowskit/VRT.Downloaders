using ReactiveUI;
using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Common.Models;
using VRT.Downloaders.Presentation.ViewModels;

namespace VRT.Downloaders.Maui.Pages;
public partial class MainPage : ContentPage, IActivatableView
{
    private readonly MainWindowViewModel _viewModel;
    private readonly IAppSettingsService _settingsService;
    private string? _lastText;
    private DateTime _lastTextDateUtc;
    public MainPage(MainWindowViewModel viewModel, IAppSettingsService settingsService)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
        _settingsService = settingsService;
        Loaded += MainPage_Loaded!;
        _settingsService.Saved += OnSettingsSaved!;
    }

    private void OnSettingsSaved(object sender, AppSettings e)
    {
        if (e.EnableClipboardMonitor != IsClipboardMonitorEnabled)
        {
            ApplySettings(e);
        }
    }

    private void ApplySettings(AppSettings settings)
    {
        if (settings.EnableClipboardMonitor)
        {
            Clipboard.ClipboardContentChanged += OnClipboardContentChanged!;
        }
        else
        {
            Clipboard.ClipboardContentChanged -= OnClipboardContentChanged!;
        }
        IsClipboardMonitorEnabled = settings.EnableClipboardMonitor;
    }

    public bool IsClipboardMonitorEnabled { get; private set; }

    private void MainPage_Loaded(object sender, EventArgs e)
    {
        ApplySettings(_settingsService.GetSettings());
    }

    private async void OnClipboardContentChanged(object sender, EventArgs e)
    {
        var text = await Clipboard.Default.GetTextAsync();
        OnClipboardTextData(text);
    }

    private void OnClipboardTextData(string? text)
    {
        var curTime = DateTime.UtcNow;
        if (string.IsNullOrWhiteSpace(text) is false && (_lastText != text || curTime.Subtract(_lastTextDateUtc).TotalSeconds > 5))
        {
            _lastText = text;
            _lastTextDateUtc = curTime;
            _viewModel.ProcessUrlCommand.Execute(text);
        }
    }
}