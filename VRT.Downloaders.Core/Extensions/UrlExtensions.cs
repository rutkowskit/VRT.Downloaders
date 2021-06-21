using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using VRT.Downloaders.Properties;
using VRT.Downloaders.Services.Downloads;

namespace VRT.Downloaders
{
    public static partial class UrlExtensions
    {
        public static async Task<Result<FilePartitions>> CreatePartitions(this Uri resourcePath)
        {
            return await GetDownloadSize(resourcePath)
                .Bind(size => CreateRanges(size).Map(r => new FilePartitions(resourcePath, size, r)));
        }

        public static async Task<Result<long>> GetDownloadSize(this Uri url)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
            var response = (HttpWebResponse)await request.GetResponseAsync();
            var length = response.ContentLength;
            return length;
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
