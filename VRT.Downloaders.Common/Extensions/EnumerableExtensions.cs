namespace VRT.Downloaders;

public static class EnumerableExtensions
{
    public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this IEnumerable<T> items)
    {
        if (items == null)
        {
            return Array.Empty<T>();
        }
        if (items is IReadOnlyCollection<T> itemsCollection)
        {
            return itemsCollection;
        }
        return items.ToArray();
    }
}