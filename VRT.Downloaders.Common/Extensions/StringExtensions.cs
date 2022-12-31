using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace VRT.Downloaders;

public static class StringExtensions
{
    private static readonly IReadOnlyCollection<char> InvalidChars;

    static StringExtensions()
    {
        InvalidChars = Path.GetInvalidFileNameChars()
            .Concat(new char[] { '"', '*', '/', ':', '<', '>', '?', '\\', '|', (char)0x7F })
            .ToHashSet();
    }
    public static string SanitizeAsFileName(this string fileName, string badCharReplacement = "_")
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return fileName;

        var result = fileName
            .Trim() //to remove whitespaces at the beginning and at the end of the file name
            .Select(c => ToSafeFileNameCharacter(c, badCharReplacement))
            .Combine();            
        return result;
    }

    public static string Format(this string format, params object?[] args)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ToSafeFileNameCharacter(this char character, string badCharReplacement)
    {
        return char.IsSurrogate(character) || char.IsControl(character) || InvalidChars.Contains(character)
            ? badCharReplacement
            : character.ToString();
    }
    private static string Combine(this IEnumerable<string> values, string delimiter="")
    {
        var result = new StringBuilder();
        var cnt = 0;
        foreach ( var value in values)
        {
            if (cnt > 0)
            {
                result.Append(delimiter);
            }                
            result.Append(value);
        }
        return result.ToString();
    }
}
