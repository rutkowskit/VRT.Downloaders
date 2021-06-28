using CSharpFunctionalExtensions;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace VRT.Downloaders
{
    public static partial class UrlExtensions
    {
        private const string _defaultUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";

        public static async Task<Result<long>> GetDownloadSize(this Uri url)
        {
            var request = CreateHttpWebRequest(url);

            var response = (HttpWebResponse)await request.GetResponseAsync();
            var length = response.ContentLength;
            return length;
        }
        public static async Task<string> GetWebFile(this Uri url, string origin = null, string referer = null)
        {
            var request = CreateHttpWebRequest(url, "GET", origin, referer);

            using (var response = request.GetResponse())
            using (var dataStream = response.GetResponseStream())
            using (var reader = new StreamReader(dataStream))
            {
                string responseFromServer = await reader.ReadToEndAsync();
                return responseFromServer;
            }
        }
        private static HttpWebRequest CreateHttpWebRequest(Uri url,
            string method = "GET",
            string origin = null, string referer = null)
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = method;
            request.UserAgent = _defaultUserAgent;

            if (!string.IsNullOrWhiteSpace(origin))
            {
                request.Headers.Add("Origin", origin);
            }
            if (!string.IsNullOrWhiteSpace(referer))
            {
                request.Referer = referer;
            }

            return request;
        }
    }
}
