using System.IO;
using VRT.Downloaders.Services.Downloads;

namespace VRT.Downloaders.Services.Medias
{
    public static class MediaInfoExtensions
    {
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
        public static DownloadRequest ToDownloadRequest(this MediaInfo media,
            string outputDirectory)
        {
            Guard.AgainstNull(media, nameof(media)).Discard();
            var fileName = string.IsNullOrWhiteSpace(media.OutputFileName)
                ? media.GetDefaultOutputFileName()
                : media.OutputFileName;
            var outFile = Path.Combine(outputDirectory, fileName);
            return new DownloadRequest(media.Title, media.Uri, outFile);
        }
    }
}
