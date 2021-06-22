using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Linq;
using System.Windows.Input;
using VRT.Downloaders.Properties;
using VRT.Downloaders.Services.Configs;

namespace VRT.Downloaders.ViewModels
{
    public sealed class SettingsViewModel : BaseViewModel
    {
        private readonly IAppSettingsService _settingsService;

        public SettingsViewModel(IAppSettingsService settingsService)
        {
            _settingsService = Guard.AgainstNull(settingsService, nameof(settingsService));

            Title = Resources.Title_Settings;
            SaveSettingsCommand = ReactiveCommand.Create(SaveSettings, CanSaveSettings());

            SetCurrentSettings(_settingsService.GetSettings());
            SetupReloadSettingsWhenSaved();
        }

        [Reactive] public AppSettings CurrentSettings { get; private set; }
        [Reactive] public string OutputDirectory { get; set; }
        [Reactive] public bool EnableClipboardMonitor { get; set; }

        public ICommand SaveSettingsCommand { get; }

        private void SetCurrentSettings(AppSettings settings)
        {
            CurrentSettings = settings;
            EnableClipboardMonitor = settings.EnableClipboardMonitor;
            OutputDirectory = settings.OutputDirectory;
        }

        private void SetupReloadSettingsWhenSaved()
        {
            _settingsService.Saved
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(SetCurrentSettings)
                .DisposeWith(Disposables)
                .Discard();
        }
        private void SaveSettings()
        {
            var newSettings = new AppSettings(OutputDirectory, EnableClipboardMonitor);
            if (!newSettings.Equals(CurrentSettings))
            {
                _settingsService.SaveSettings(newSettings);
            }
        }
        private IObservable<bool> CanSaveSettings()
        {
            return this.WhenAnyValue(p => p.OutputDirectory, 
                    p => p.EnableClipboardMonitor, p => p.CurrentSettings)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(_ => HasChanges());
        }
        private bool HasChanges()
        {
            var current = CurrentSettings;
            var toCompare = new AppSettings(OutputDirectory, EnableClipboardMonitor);
            return current != null && !toCompare.Equals(current);
        }
    }
}
