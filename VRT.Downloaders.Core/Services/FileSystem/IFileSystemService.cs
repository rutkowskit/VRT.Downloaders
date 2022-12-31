namespace VRT.Downloaders.Services.FileSystem;

public interface IFileSystemService
{
    string GetCurrentDirectory();
    string GetAppDataDirectory(bool ensureCreated);
    string GetDownloadsDirectory(bool ensureCreated);
}
