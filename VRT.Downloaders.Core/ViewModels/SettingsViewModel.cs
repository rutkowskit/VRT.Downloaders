using VRT.Downloaders.Extensions;
using VRT.Downloaders.Services.Confirmation;
using VRT.Downloaders.Services.FileSystem;

namespace VRT.Downloaders.ViewModels;

public sealed class SettingsViewModel : BaseViewModel
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
        SaveSettingsCommand = ReactiveCommand.Create(SaveSettings, CanSaveSettings());
        ResetSettingsCommand = ReactiveCommand.CreateFromTask(ResetSettings);
        PickOutputDirectoryCommand = ReactiveCommand.CreateFromTask(PickOutputDirectory, CanPickOutputDirectory())
            .WithDevNullExceptionHandler();            
        SetCurrentSettings(_settingsService.GetSettings());
        _settingsService.Saved += OnSettingsSaved;
        _confirmationService = confirmationService;
        _folderPicker = folderPicker;
        IsFolderPickerSupported = _folderPicker.IsPickFolderSupported;
    }

    [Reactive] public AppSettings CurrentSettings { get; private set; }
    [Reactive] public string OutputDirectory { get; set; }
    [Reactive] public bool EnableClipboardMonitor { get; set; }
    [Reactive] public bool EnableAutoGetMedias { get; set; }
    [Reactive] public bool IsFolderPickerSupported { get; private set; }
    [Reactive] public string AutoDownloadMediaTypePattern { get; set; }

    public ICommand SaveSettingsCommand { get; }
    public ICommand ResetSettingsCommand { get; }
    public ICommand PickOutputDirectoryCommand { get; }

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

    private async Task ResetSettings()
    {
        if (await _confirmationService.Confirm("Are you sure you want to reset settings ?", "Conform") is false)
        {
            return;
        }
        await _settingsService.ResetToDefaults();
    }
    private void SaveSettings()
    {
        var newSettings = CreateCurrentAppSettings();
        if (!newSettings.Equals(CurrentSettings))
        {
            _settingsService.SaveSettings(newSettings);
        }
    }
    private IObservable<bool> CanSaveSettings()
    {
        return this
            .WhenAnyPropertyChanged()
            .Throttle(TimeSpan.FromMilliseconds(200))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => HasChanges());
    }
    private bool HasChanges()
    {
        var current = CurrentSettings;
        var toCompare = CreateCurrentAppSettings();
        return current != null && !toCompare.Equals(current);
    }
    private AppSettings CreateCurrentAppSettings() => new (OutputDirectory, EnableClipboardMonitor, EnableAutoGetMedias, AutoDownloadMediaTypePattern);
    private async Task PickOutputDirectory()
    {
        await _folderPicker.PickFolder()
            .Tap(folder => OutputDirectory = folder);
    }
    private IObservable<bool> CanPickOutputDirectory()
    {
        return this
            .WhenAnyValue(p => p.IsFolderPickerSupported)
            .ObserveOn(RxApp.MainThreadScheduler);
    }
}
