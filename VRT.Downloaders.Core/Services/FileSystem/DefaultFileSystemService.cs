namespace VRT.Downloaders.Services.FileSystem
{
    public sealed class DefaultFilesystemService : IFileSystemService
    {
        public string GetAppDataDirectory(bool ensureCreated)
            => DirectoryHelper.GetAppDataDirectory(ensureCreated);

        public string GetCurrentDirectory()
            => DirectoryHelper.GetCurrentDirectory();

        public string GetDownloadsDirectory(bool ensureCreated)
            => DirectoryHelper.GetUserDownloadsDirectory(ensureCreated);
    }
}
