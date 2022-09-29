namespace VRT.Downloaders.Services.FileSystem;
public interface IFolderPickerService
{
    Task<Result<string>> PickFolder();
    bool IsPickFolderSupported { get; }
}
