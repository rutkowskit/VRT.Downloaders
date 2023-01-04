namespace VRT.Assets.Application.Common.Abstractions;
public interface IAbstractFactory<T>    
{
    public T Create();
}
