using System.ComponentModel;
using VRT.Downloaders.Common.Models;

namespace VRT.Downloaders.Maui.ViewModels;

public sealed partial class SettingsViewModel : BaseViewModel
{
    private readonly IAppSettingsService _settingsService;
    private readonly IConfirmationService _confirmationService;
    private readonly IFolderPickerService _folderPicker;

    public SettingsViewModel(
        IAppSettingsService settingsService,
        IConfirmationService confirmationService,
        IFolderPickerService folderPicker)
    {
        _settingsService = Guard.AgainstNull(settingsService, nameof(settingsService));
        Title = Resources.Title_Settings;
        SetCurrentSettings(_settingsService.GetSettings());
        _settingsService.Saved += OnSettingsSaved!;
        _confirmationService = confirmationService;
        _folderPicker = folderPicker;
        IsFolderPickerSupported = _folderPicker.IsPickFolderSupported;
    }

    [ObservableProperty] public partial AppSettings? CurrentSettings { get; set; }
    [ObservableProperty] public partial string? OutputDirectory { get; set; }
    [ObservableProperty] public partial bool EnableClipboardMonitor { get; set; }
    [ObservableProperty] public partial bool EnableAutoGetMedias { get; set; }
    [ObservableProperty] public partial bool IsFolderPickerSupported { get; set; }
    [ObservableProperty] public partial string? AutoDownloadMediaTypePattern { get; set; }

    private void SetCurrentSettings(AppSettings settings)
    {
        CurrentSettings = settings;
        EnableClipboardMonitor = settings.EnableClipboardMonitor;
        OutputDirectory = settings.OutputDirectory;
        EnableAutoGetMedias = settings.EnableAutoGetMedias;
        AutoDownloadMediaTypePattern = settings.AutoDownloadMediaTypePattern;
    }

    private void OnSettingsSaved(object sender, AppSettings e)
    {
        SetCurrentSettings(e);
    }

    [RelayCommand]
    private async Task ResetSettings()
    {
        if (await _confirmationService.Confirm("Are you sure you want to reset settings ?", "Conform") is false)
        {
            return;
        }
        await _settingsService.ResetToDefaults();
    }

    [RelayCommand(CanExecute = nameof(HasChanges))]
    private void SaveSettings()
    {
        var newSettings = CreateCurrentAppSettings();
        if (!newSettings.Equals(CurrentSettings))
        {
            _settingsService.SaveSettings(newSettings);
        }
    }

    [RelayCommand(CanExecute = nameof(CanPickOutputDirectory))]
    private async Task PickOutputDirectory()
    {
        await _folderPicker.PickFolder()
            .TapOnDispatcher(folder => OutputDirectory = folder);
    }
    private bool CanPickOutputDirectory()
    {
        return this.GetOnDispatcher(v => v.IsFolderPickerSupported);
    }
    private bool HasChanges()
    {
        var current = CurrentSettings;
        var toCompare = CreateCurrentAppSettings();
        return current != null && !toCompare.Equals(current);
    }
    private AppSettings CreateCurrentAppSettings()
    {
        var result = new AppSettings(OutputDirectory!, EnableClipboardMonitor, EnableAutoGetMedias, AutoDownloadMediaTypePattern);
        return result;
    }
    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        await this.DoOnDispatcher(p =>
        {
            p.SaveSettingsCommand.NotifyCanExecuteChanged();
            p.ResetSettingsCommand.NotifyCanExecuteChanged();
            p.PickOutputDirectoryCommand.NotifyCanExecuteChanged();
        });
    }
}
