using System.Reactive.Disposables;
using VRT.Downloaders.Common.Collections;

namespace VRT.Downloaders.Infrastructure.DownloadExecutor;

public sealed class RemoteStream : IDisposable
{
    private readonly CompositeDisposable _disposables;
    private readonly HttpClient _httpClient;
    public RemoteStream(Uri url, FileByteRange range)
    {
        _disposables = [];
        Url = url;
        Range = range;

        _httpClient = new(new HttpClientHandler
        {
            MaxConnectionsPerServer = 1000 // Set the maximum number of connections per server                            
        });
        _httpClient.DisposeWith(_disposables);
    }

    public Uri Url { get; }
    public FileByteRange Range { get; }

    public void Dispose()
    {
        _disposables.Dispose();
    }

    public async Task<Stream> Open()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, Url);
        request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(Range.From, Range.To);
        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        return stream.SetDisposable(_disposables);
    }
}
