using System.IO;
using VRT.Downloaders.Services.FileSystem;

namespace VRT.Downloaders.Mobile.iOS.Services
{
    public sealed class IosFileSystemService : IFileSystemService
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
            var myDocuments = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            return Path.Combine(myDocuments, "Downloads");
        }
    }
}