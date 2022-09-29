using MediatR;
using VRT.Downloaders.Services.Navigation;
using CSharpFunctionalExtensions;
using VRT.Downloaders.Maui.Pages;

namespace VRT.Downloaders.Maui.Events;
public sealed class ShowErrorRequestHandler : IRequestHandler<ShowErrorRequest,Result>
{
    private readonly AppShell _shell;

    public ShowErrorRequestHandler(AppShell shell)
    {
        _shell = shell;
    }

    public async Task<Result> Handle(ShowErrorRequest request, CancellationToken cancellationToken)
    {
        var route = nameof(ErrorPage);
        var routeParams = new Dictionary<string, object>
        {
            ["ErrorMessage"] = request.ErrorMessage
        };
        var result = await Result.Try(() => _shell.GoToAsync(route, routeParams));
        return result;        
    }
}
