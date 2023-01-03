using MediatR;

namespace VRT.Downloaders.Common.Requests;
public sealed record PickFolderRequest() : IRequest<Result<string>>;
