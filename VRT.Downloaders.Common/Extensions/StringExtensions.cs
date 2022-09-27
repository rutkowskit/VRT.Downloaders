using System;
using System.IO;
using System.Linq;

namespace VRT.Downloaders
{
    public static class StringExtensions
    {
        private static readonly char[] InvalidChars;

        static StringExtensions()
        {
            InvalidChars = Path.GetInvalidFileNameChars()
                .Concat(new char[] { '"', '*', '/', ':', '<', '>', '?', '\\', '|', (char)0x7F })
                .Distinct()
                .ToArray();
        }
        public static string SanitizeAsFileName(this string escapedString, string badCharReplacement = "_")
        {
            if (string.IsNullOrWhiteSpace(escapedString))
                return escapedString;

            var newName = string
                .Join(badCharReplacement, escapedString.Split(InvalidChars, StringSplitOptions.RemoveEmptyEntries))
                .TrimEnd('.');
            return newName;
        }

        public static string Format(this string format, params object[] args)
        {
            return string.IsNullOrWhiteSpace(format) || args == null || args.Length == 0
                ? format
                : string.Format(format, args);
        }

        public static string GetMediaExtension(this string uri)
        {
            var extension = Path.GetExtension(uri);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = "unknown";
            }
            return extension;
        }
    }
}
