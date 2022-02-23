using System.Collections.Generic;
using System.Linq;

namespace VRT.Downloaders
{
    public static class EnumerableExtensions
    {
        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this IEnumerable<T> items)
        {
            if (items == null)
            {
                return null;
            }
            if (items is IReadOnlyCollection<T> itemsCollection)
            {
                return itemsCollection;
            }
            return items.ToArray();
        }
    }
}