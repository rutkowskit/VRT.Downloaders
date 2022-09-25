using X=Android.OS;
using VRT.Downloaders.Services.FileSystem;

namespace VRT.Downloaders.Maui.Platforms.Android.Services;

public sealed class AndroidFileSystemService : IFileSystemService
{
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
        var result = X.Environment.IsExternalStorageEmulated
            ? Path.Combine(X.Environment.ExternalStorageDirectory.AbsolutePath, X.Environment.DirectoryDownloads)
            : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return result;
        //var result = (string)Android.OS.Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads);
        //return result;
    }
}