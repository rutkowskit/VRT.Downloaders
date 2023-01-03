using VRT.Downloaders.Common.Abstractions;

namespace VRT.Downloaders.Fakes;
internal sealed class FakeAppStateService : IAppStateService
{
    private readonly Dictionary<string,object?> _cache= new();

    public T? Restore<T>(string key)
    {
        return _cache.TryGetValue(key, out var value) && value is T tValue
            ? tValue 
            : default;
    }

    public void Store<T>(string key, T value)
    {
        _cache[key] = value;
    }
}
