using MediatR;
using VRT.Downloaders.Extensions;
using VRT.Downloaders.Services.Medias;

namespace VRT.Downloaders.Medias.Queries.GetMedias;
public sealed record GetMediasQuery(string Uri) : IRequest<Result<MediaInfo[]>>
{    
    internal sealed class GetMediasQueryHandler : IRequestHandler<GetMediasQuery, Result<MediaInfo[]>>
    {
        private readonly IReadOnlyCollection<IMediaService> _mediaServices;

        public GetMediasQueryHandler(IEnumerable<IMediaService> mediaServices)
        {
            _mediaServices = mediaServices.OrderBy(o=>o.GetServicePriority()).ToArray();
        }
        public async Task<Result<MediaInfo[]>> Handle(GetMediasQuery request, CancellationToken cancellationToken)
        {
            var uri = request.Uri;
            var result = await _mediaServices.GetMediaService(uri)
                .BindTry(mediaService => mediaService.GetAvailableMedias(uri))
                .Tap(medias => medias.SetDefaultOutputFileName());
            return result;
        }        
    }
}
