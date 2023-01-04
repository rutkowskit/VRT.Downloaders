using VRT.Assets.Application.Common.Abstractions;

namespace VRT.Downloaders.Common.Factories;
public sealed class AbstractFactory<T> : IAbstractFactory<T>
{
    private readonly Func<T> _factory;

    public AbstractFactory(Func<T> factory)
    {
        _factory = factory;
    }
    public T Create()
    {
        return _factory();
    }
}
