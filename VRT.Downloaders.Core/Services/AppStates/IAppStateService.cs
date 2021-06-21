namespace VRT.Downloaders.Services.AppStates
{
    public interface IAppStateService
    {
        void Store<T>(string key, T value);
        T Restore<T>(string key);
    }
}
