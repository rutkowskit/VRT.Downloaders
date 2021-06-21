namespace VRT.Downloaders.Services.Configs
{
    public interface IAppSettingsService
    {
        AppSettings GetSettings();
        void SaveSettings(AppSettings settings);
    }
}
