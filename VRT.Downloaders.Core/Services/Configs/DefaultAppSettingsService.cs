using Newtonsoft.Json;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using VRT.Downloaders.Services.FileSystem;

namespace VRT.Downloaders.Services.Configs
{
    public sealed class DefaultAppSettingsService : IAppSettingsService
    {
        private const string _settingsFileName = "AppSettings.json";
        private readonly string _settingsFilePath;
        private readonly IFileSystemService _fileSystemService;
        private AppSettings _currentSettings;
        private readonly ISubject<AppSettings> _savedSubject;

        public IObservable<AppSettings> Saved { get; }

        public DefaultAppSettingsService(IFileSystemService fileSystemService)
        {
            _savedSubject = new Subject<AppSettings>();
            Saved = _savedSubject.AsObservable();
            _fileSystemService = fileSystemService;
            _settingsFilePath = GetSettingsFilePath();
        }

        public AppSettings GetSettings()
        {
            if (_currentSettings != null)
                return _currentSettings;

            return _currentSettings = LoadSettings(_settingsFilePath);
        }

        public void SaveSettings(AppSettings settings)
        {
            if (HasChanges(settings))
            {
                SaveSettingsToFile(settings, _settingsFilePath);
                _currentSettings = settings;
                _savedSubject.OnNext(settings);                
            };
        }

        private bool HasChanges(AppSettings settings)
        {
            var current = GetSettings();
            return settings.EnableClipboardMonitor != current.EnableClipboardMonitor
                || settings.OutputDirectory != current.OutputDirectory;
        }

        private string GetSettingsFilePath()
        {
            var appDataDir = _fileSystemService.GetAppDataDirectory(true);
            return Path.Combine(appDataDir, _settingsFileName);
        }

        private AppSettings LoadSettings(string configFilePath)
        {
            return File.Exists(configFilePath)
                ? LoadSettingsFromFile(configFilePath)
                : GetDefaultSettings();
        }

        private AppSettings GetDefaultSettings()
        {
            var outputDirectory = _fileSystemService.GetDownloadsDirectory(true);
            return new AppSettings(outputDirectory, false);
        }

        private static AppSettings LoadSettingsFromFile(string configFilePath)
        {
            var content = File.ReadAllText(configFilePath, Encoding.UTF8);
            return JsonConvert.DeserializeObject<AppSettings>(content);
        }

        private static void SaveSettingsToFile(AppSettings settings, string configFilePath)
        {
            var content = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(configFilePath, content, Encoding.UTF8);
        }
    }
}
