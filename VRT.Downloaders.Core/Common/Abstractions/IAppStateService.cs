namespace VRT.Downloaders.Common.Abstractions;

public interface IAppStateService
{
    void Store<T>(string key, T value);
    T? Restore<T>(string key);
}
