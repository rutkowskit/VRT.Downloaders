using System.Runtime.CompilerServices;

namespace VRT.Downloaders;

public static class Guard
{
    public static T AgainstNull<T>(this T? field,
        [CallerArgumentExpression("field")] string? fieldName = null,
        string? message = null)
        where T : class
    {
        return field ?? throw new ArgumentNullException(fieldName, message);
    }
}
