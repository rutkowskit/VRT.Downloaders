using System;
using System.IO;

namespace VRT.Downloaders
{
    public static class StringExtensions
    {
        private static readonly char[] _invalids;

        static StringExtensions()
        {
            _invalids = System.IO.Path.GetInvalidFileNameChars();
        }
        public static string SanitizeAsFileName(this string escapedString, string badCharReplacement = "_")
        {
            if (string.IsNullOrWhiteSpace(escapedString))
                return escapedString;

            var newName = string
                .Join(badCharReplacement, escapedString.Split(_invalids, StringSplitOptions.RemoveEmptyEntries))
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
