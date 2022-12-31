using System.Collections;
using System.Threading;

namespace VRT.Downloaders.Services.Downloads;

/// <summary>
/// Thread safe list
/// </summary>
/// <typeparam name="T">List type parameter</typeparam>
public sealed class BlockingList<T> : IList<T>
{
    private readonly List<T> _list;
    private readonly SemaphoreSlim _semaphore;
    public BlockingList()
    {
        _list = new List<T>();
        _semaphore = new SemaphoreSlim(1, 1);
    }
    public T this[int index]
    {
        get => PerformBlockingOperation(l => l[index]);
        set => PerformBlockingOperation(l => l[index] = value);
    }

    public int Count => PerformBlockingOperation(l => l.Count);
    public bool IsReadOnly => false;
    public void Add(T item)
    {
        PerformBlockingOperation(l => l.Add(item));
    }

    public void Clear()
    {
        PerformBlockingOperation(l => l.Clear());
    }

    public bool Contains(T item)
    {
        return PerformBlockingOperation(l => l.Contains(item));
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        PerformBlockingOperation(l => l.CopyTo(array, arrayIndex));
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return PerformBlockingOperation(l => l.IndexOf(item));
    }

    public void Insert(int index, T item)
    {
        PerformBlockingOperation(l => l.Insert(index, item));
    }

    public bool Remove(T item)
    {
        return PerformBlockingOperation(l => l.Remove(item));
    }

    public void RemoveAt(int index)
    {
        PerformBlockingOperation(l => l.RemoveAt(index));
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    private void PerformBlockingOperation(Action<List<T>> action)
    {
        try
        {
            _semaphore.Wait();
            action(_list);
        }
        finally
        {
            _semaphore.Release();
        }
    }
    private TResponse PerformBlockingOperation<TResponse>(Func<List<T>, TResponse> action)
    {
        try
        {
            _semaphore.Wait();
            return action(_list);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
