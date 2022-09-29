namespace VRT.Downloaders
{
    public static partial class UrlExtensions
    {
        public static async Task<Result<FilePartitions>> CreatePartitions(this Uri resourcePath)
        {
            return await resourcePath.GetDownloadSize()
                .Bind(size => CreateRanges(size).Map(r => new FilePartitions(resourcePath, size, r)));
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
