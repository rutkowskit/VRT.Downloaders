namespace VRT.Downloaders.Common.Abstractions;

public interface IFileSystemService
{
    string GetCurrentDirectory();
    string GetAppDataDirectory(bool ensureCreated);
    string GetDownloadsDirectory(bool ensureCreated);
    bool FileExists(string path);
}
