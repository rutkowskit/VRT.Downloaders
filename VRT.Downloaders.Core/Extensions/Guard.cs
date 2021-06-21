using System;

namespace VRT.Downloaders
{
    public static class Guard
    {
        public static T AgainstNull<T>(this T field, string fieldName)
            where T : class
        {
            return field ?? throw new ArgumentNullException(fieldName);
        }
    }
}
