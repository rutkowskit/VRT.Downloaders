using MediatR;

namespace VRT.Downloaders.Common.Requests;
public sealed record ShowErrorRequest(string ErrorMessage) : IRequest<Result>;
