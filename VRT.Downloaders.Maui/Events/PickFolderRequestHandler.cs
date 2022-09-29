using CSharpFunctionalExtensions;
using MediatR;
using VRT.Downloaders.Maui.Helpers;
using VRT.Downloaders.Services.Navigation;

namespace VRT.Downloaders.Maui.Events;
internal class PickFolderRequestHandler : IRequestHandler<PickFolderRequest, Result<string>>
{
    public async Task<Result<string>> Handle(PickFolderRequest request, CancellationToken cancellationToken)
    {
        var result = await FolderHelpers.PickFolder();
        return result;
    }
}
