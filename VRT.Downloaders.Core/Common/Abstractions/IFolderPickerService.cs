namespace VRT.Downloaders.Common.Abstractions;
public interface IFolderPickerService
{
    Task<Result<string>> PickFolder();
    bool IsPickFolderSupported { get; }
}
