using Newtonsoft.Json;
using System.IO;
using VRT.Downloaders.Services.FileSystem;

namespace VRT.Downloaders.Services.Configs;

public sealed class DefaultAppSettingsService : IAppSettingsService
{
    private const string SettingsFileName = "AppSettings.json";

    private readonly string _settingsFilePath;
    private readonly IFileSystemService _fileSystemService;
    private AppSettings _currentSettings;

    public event EventHandler<AppSettings> Saved;

    public DefaultAppSettingsService(IFileSystemService fileSystemService)
    {
        Saved = delegate { };
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
            Saved(this, settings);
        };
    }
    public async Task ResetToDefaults()
    {
        await Task.CompletedTask;
        var defaults = GetDefaultSettings();
        SaveSettings(defaults);
    }

    private bool HasChanges(AppSettings settings)
    {
        var current = GetSettings();
        return settings.Equals(current) is false;            
    }

    private string GetSettingsFilePath()
    {
        var appDataDir = _fileSystemService.GetAppDataDirectory(true);
        return Path.Combine(appDataDir, SettingsFileName);
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
        return new AppSettings(outputDirectory, false, false, null);
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
