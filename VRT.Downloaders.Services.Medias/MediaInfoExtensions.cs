using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VRT.Downloaders.Services.Medias;
public static class MediaInfoExtensions
{
    public static Result<MediaInfo> FindFirstByDescription(this IEnumerable<MediaInfo> medias, string regexPattern)
    {
        Guard.AgainstNull(medias);
        var result = medias
            .Where(m => m?.FormatDescription is not null)
            .Where(m => Regex.IsMatch(m.FormatDescription, regexPattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase))
            .FirstOrDefault();
        return result ?? Result.Failure<MediaInfo>("Media not found");
    }
    public static void SetDefaultOutputFileName(this MediaInfo[] medias)
    {
        if (medias == null || medias.Length == 0)
        {
            return;
        }
        foreach (var media in medias)
        {
            media.OutputFileName = media.GetDefaultOutputFileName();
        }
    }
    public static string GetDefaultOutputFileName(this MediaInfo media)
    {
        return media == null
            ? "unknown.bin"
            : $"{media.Title}.{media.Extension}".SanitizeAsFileName();
    }
}
