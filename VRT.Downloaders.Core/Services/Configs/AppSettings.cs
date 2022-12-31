namespace VRT.Downloaders.Services.Configs;

public sealed class AppSettings : ValueObject
{
    public AppSettings(
        string outputDirectory, 
        bool enableClipboardMonitor,
        bool enableAutoGetMedias,
        string? autoDownloadMediaTypePattern)
    {
        EnableClipboardMonitor = enableClipboardMonitor;
        OutputDirectory = outputDirectory;
        EnableAutoGetMedias = enableAutoGetMedias;
        AutoDownloadMediaTypePattern = autoDownloadMediaTypePattern;
    }
    public bool EnableClipboardMonitor { get; }
    public bool EnableAutoGetMedias { get; }
    public string OutputDirectory { get; }
    /// <summary>
    /// Regex pattern of Type name to automaticly queue for download after getting media list
    /// e.g. ^Audio.*?mp4 will cause automatically start downloading audio in mp4 format after media list is refreshed
    /// </summary>
    public string? AutoDownloadMediaTypePattern { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EnableClipboardMonitor;
        yield return OutputDirectory;
        yield return EnableAutoGetMedias;
        yield return AutoDownloadMediaTypePattern ?? "";
    }
}
