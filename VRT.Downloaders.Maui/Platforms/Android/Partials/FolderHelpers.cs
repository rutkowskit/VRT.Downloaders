using CSharpFunctionalExtensions;

namespace VRT.Downloaders.Maui.Helpers;
internal static partial class FolderHelpers
{
    public static partial async Task<Result<string>> PickFolder()
    {
        await Task.CompletedTask;
        return Result.Failure<string>("Picking folder is not supported");        
    }
}
