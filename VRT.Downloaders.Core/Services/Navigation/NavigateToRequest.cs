using MediatR;

namespace VRT.Downloaders.Services.Navigation;
public sealed record ShowErrorRequest(string ErrorMessage) : IRequest<Result>;
