namespace VRT.Downloaders.Common.Models;
public sealed record DownloadRequest(string Name, Uri Uri, string OutputFileName);