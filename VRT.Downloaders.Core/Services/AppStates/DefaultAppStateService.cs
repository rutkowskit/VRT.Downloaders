using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace VRT.Downloaders.Services.AppStates
{
    public sealed class DefaultAppStateService : IAppStateService
    {
        public T Restore<T>(string key)
        {
            var inputFile = GetFilePathByKey(key);
            if (!File.Exists(inputFile))
            {
                return default;
            }
            var json = File.ReadAllText(inputFile, Encoding.UTF8);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Store<T>(string key, T value)
        {
            var outputFile = GetFilePathByKey(key);
            var json = JsonConvert.SerializeObject(value);
            File.WriteAllText(outputFile, json, Encoding.UTF8);
        }
        private static string GetFilePathByKey(string key)
        {
            var fileName = $"{key.SanitizeAsFileName()}.json";
            return Path.Combine(DirectoryHelper.GetAppDataDirectory(), fileName);
        }
    }
}
