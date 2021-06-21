using Android.OS;
using System.IO;
using VRT.Downloaders.Services.FileSystem;

namespace VRT.Downloaders.Mobile.Droid.Services
{
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
            var result = Environment.IsExternalStorageEmulated
                ? Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads)
                : System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            return result;
            //var result = (string)Android.OS.Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads);
            //return result;
        }
    }
}