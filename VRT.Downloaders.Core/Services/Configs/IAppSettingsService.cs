using System;

namespace VRT.Downloaders.Services.Configs
{
    public interface IAppSettingsService
    {        
        IObservable<AppSettings> Saved { get; }
        AppSettings GetSettings();
        void SaveSettings(AppSettings settings);
    }
}
