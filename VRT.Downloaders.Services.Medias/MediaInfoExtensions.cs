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
    }
}
