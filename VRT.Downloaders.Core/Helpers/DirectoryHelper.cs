using System;
using System.IO;
using System.Reflection;
using VRT.Downloaders.Services.Configs;

namespace VRT.Downloaders
{
    public static class DirectoryHelper
    {
        public static string GetCurrentDirectory()
        {
            var uriPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var result = new Uri(uriPath).LocalPath;
            return result;
        }
        public static string GetAppDataDirectory(bool ensureCreated = true)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var subfolder = typeof(DefaultAppSettingsService).Assembly.GetName().Name;
            var fullPath = Path.Combine(appData, subfolder);
            return ensureCreated
                ? Directory.CreateDirectory(fullPath).FullName
                : fullPath;
        }

        public static string GetUserDownloadsDirectory(bool ensureCreated = true)
        {            
            var downloads = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            var fullPath = Path.Combine(downloads, "Downloads");
            return ensureCreated
                ? Directory.CreateDirectory(fullPath).FullName
                : fullPath;
        }
    }
}
