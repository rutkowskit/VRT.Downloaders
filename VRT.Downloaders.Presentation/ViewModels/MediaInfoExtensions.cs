using VRT.Downloaders.Common.Models;
using VRT.Downloaders.Services.Medias;

namespace VRT.Downloaders.Infrastructure.Extensions;
public static class MediaInfoExtensions
{
    public static DownloadRequest ToDownloadRequest(this MediaInfo media,
        string outputDirectory)
    {
        media.AgainstNull(nameof(media)).Discard();
        var fileName = string.IsNullOrWhiteSpace(media.OutputFileName)
            ? media.GetDefaultOutputFileName()
            : media.OutputFileName;
        var outFile = Path.Combine(outputDirectory, fileName);
        return new DownloadRequest(media.Title, media.Url, outFile);
    }
}
