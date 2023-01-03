using CSharpFunctionalExtensions;
using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Maui.Helpers;

namespace VRT.Downloaders.Maui.Services;
internal sealed class FolderPickerService : IFolderPickerService
{
    public bool IsPickFolderSupported 
    { 
        get
        {
#if WINDOWS
            return true;
#else
            return false;
#endif
        } 
    }

    public async Task<Result<string>> PickFolder()
    {
        return await FolderHelpers.PickFolder();
    }
}
