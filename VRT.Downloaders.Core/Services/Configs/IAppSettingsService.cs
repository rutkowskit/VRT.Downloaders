namespace VRT.Downloaders.Services.Configs;

public interface IAppSettingsService
{
    event EventHandler<AppSettings> Saved;
    AppSettings GetSettings();
    void SaveSettings(AppSettings settings);
    Task ResetToDefaults();
}
