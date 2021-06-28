using System.IO;
using VRT.Downloaders.Services.Downloads;
using VRT.Downloaders.Services.Medias;

namespace VRT.Downloaders.Services.Downloads
{
    public static class MediaInfoExtensions
    {
        public static DownloadRequest ToDownloadRequest(this MediaInfo media,
            string outputDirectory)
        {
            Guard.AgainstNull(media, nameof(media)).Discard();
            var fileName = string.IsNullOrWhiteSpace(media.OutputFileName)
                ? media.GetDefaultOutputFileName()
                : media.OutputFileName;
            var outFile = Path.Combine(outputDirectory, fileName);
            return new DownloadRequest(media.Title, media.Url, outFile);
        }
    }
}
