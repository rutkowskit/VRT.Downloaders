namespace VRT.Downloaders.Services.Downloads;
public sealed record DownloadRequest(string Name, Uri Uri, string OutputFileName);