using VRT.Downloaders.Common.Models;

namespace VRT.Downloaders.Common.Abstractions;

public interface IAppSettingsService
{
    event EventHandler<AppSettings> Saved;
    AppSettings GetSettings();
    void SaveSettings(AppSettings settings);
    Task ResetToDefaults();
}
