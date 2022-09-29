using MediatR;

namespace VRT.Downloaders.Services.Navigation;
public sealed record PickFolderRequest() : IRequest<Result<string>>;
