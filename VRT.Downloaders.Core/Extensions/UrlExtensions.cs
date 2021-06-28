﻿using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using VRT.Downloaders.Properties;
using VRT.Downloaders.Services.Downloads;

namespace VRT.Downloaders
{
    public static partial class UrlExtensions
    {
        private const string _defaultUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
        public static async Task<Result<FilePartitions>> CreatePartitions(this Uri resourcePath)
        {
            return await GetDownloadSize(resourcePath)
                .Bind(size => CreateRanges(size).Map(r => new FilePartitions(resourcePath, size, r)));
        }

        public static async Task<Result<long>> GetDownloadSize(this Uri url)
        {
            var request = CreateHttpWebRequest(url);

            var response = (HttpWebResponse)await request.GetResponseAsync();
            var length = response.ContentLength;
            return length;
        }
        public static async Task<string> GetWebFile(this Uri url, string origin = null, string referer = null)
        {
            var request = CreateHttpWebRequest(url, origin, referer);                

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

        private static Result<FileByteRange[]> CreateRanges(long size)
        {
            if (size <= 0)
                return Result.Failure<FileByteRange[]>(Resources.Error_SizeMustBeGreaterThanZero);

            const int bufferSize = 5 * 1024 * 1024;
            var remainingSize = size;
            var partitions = new List<FileByteRange>();
            long from = 0;
            while (remainingSize > 0)
            {
                var partitionSize = remainingSize > bufferSize
                    ? bufferSize
                    : remainingSize;
                var to = from + partitionSize;
                partitions.Add(new FileByteRange(from, to));
                from = to + 1;
                remainingSize -= partitionSize;
            }
            return partitions.ToArray();
        }
    }
}
