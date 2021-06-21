using System.Threading.Tasks;

namespace VRT.Downloaders
{
    public static class ObjectExtensions
    {
        public static void Discard<T>(this T toDiscard)
        {
            _ = toDiscard;
        }

        public static async Task Discard<T>(this Task<T> toDiscard)
        {
            _ = await toDiscard;
        }
    }
}
