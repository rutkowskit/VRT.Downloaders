using VRT.Downloaders.Common.Abstractions;
using X = Android.OS;

namespace VRT.Downloaders.Maui.Platforms.Android.Services;

public sealed class AndroidFileSystemService : IFileSystemService
{
    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public string GetAppDataDirectory(bool ensureCreated)
    {
        return DirectoryHelper.GetAppDataDirectory(ensureCreated);
    }

    public string GetCurrentDirectory()
    {
        return DirectoryHelper.GetCurrentDirectory();
    }

    public string GetDownloadsDirectory(bool ensureCreated)
    {
#pragma warning disable  // Type or member is obsolete
        var result = X.Environment.IsExternalStorageEmulated
            ? Path.Combine(X.Environment.ExternalStorageDirectory.AbsolutePath, X.Environment.DirectoryDownloads)
            : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return result;
#pragma warning restore SYSLIB0014 // Type or member is obsolete
    }
}