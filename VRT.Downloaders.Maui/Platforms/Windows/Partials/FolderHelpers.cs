using CSharpFunctionalExtensions;
using System.Diagnostics;
using Windows.Storage.Pickers;

namespace VRT.Downloaders.Maui.Helpers;
internal static partial class FolderHelpers
{
    public static partial async Task<Result<string>> PickFolder()
    {
        var result = await Result.Success(new FolderPicker())
            .Tap(fp => fp.FileTypeFilter.Add("*"))
            .Tap(AssociateWithMainWindow)
            .MapTry(async fp => await fp.PickSingleFolderAsync())
            .Map(dir => dir?.Path)
            .Ensure(path => string.IsNullOrWhiteSpace(path) is false, "Operation canceled");
        return result;
    }

    private static void AssociateWithMainWindow(FolderPicker picker)
    {
        var currentProcess = Process.GetCurrentProcess();
        var hwnd = currentProcess.MainWindowHandle;
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
    }
}
