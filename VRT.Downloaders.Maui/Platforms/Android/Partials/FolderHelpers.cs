using Android.Content;
using Android.Provider;
using AndroidX.DocumentFile.Provider;
using CSharpFunctionalExtensions;
using VRT.Downloaders.Maui.Platforms.Android.Partials;

namespace VRT.Downloaders.Maui.Helpers;
internal static partial class FolderHelpers
{
    private const int PickFolderRequestCode = 692348;

    public static partial async Task<Result<string>> PickFolder()
    {
        var result = await Result.Try(() => PickFolderAsync());
        return result;
    }
    static async Task<string> PickFolderAsync()
    {
        var action = Intent.ActionOpenDocumentTree;
        var intent = new Intent(action);

        var pickerIntent = Intent.CreateChooser(intent, "Select folder");

        try
        {
            var result = string.Empty;
            var x = await ProxyActivity.StartAsync(pickerIntent, PickFolderRequestCode, null, i => result = OnResult(i));
            return result;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }
    private static string OnResult(Intent intent)
    {
        if (intent.Data is not null)
        {
            var path = EnsurePhysicalPath(intent.Data, true);
            return path;
        }
        return null;
    }
    public static string EnsurePhysicalPath(Android.Net.Uri uri, bool requireExtendedAccess = true)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        var root = global::Android.OS.Environment.ExternalStorageDirectory.Path;
#pragma warning restore CS0618 // Type or member is obsolete
        var docId = DocumentsContract.GetTreeDocumentId(uri);

        var docFile = DocumentFile.FromTreeUri(Android.App.Application.Context, uri);
        return docFile.Uri.Path;
    }
}
