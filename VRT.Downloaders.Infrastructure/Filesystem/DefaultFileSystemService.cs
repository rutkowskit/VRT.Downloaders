using VRT.Downloaders.Common.Abstractions;

namespace VRT.Downloaders.Abstractions.FileSystem;

public sealed class DefaultFileSystemService : IFileSystemService
{
    public string GetAppDataDirectory(bool ensureCreated)
        => DirectoryHelper.GetAppDataDirectory(ensureCreated);

    public string GetCurrentDirectory()
        => DirectoryHelper.GetCurrentDirectory();

    public string GetDownloadsDirectory(bool ensureCreated)
        => DirectoryHelper.GetUserDownloadsDirectory(ensureCreated);
}
