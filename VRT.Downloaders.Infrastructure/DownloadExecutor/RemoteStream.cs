using System.Net;
using System.Reactive.Disposables;
using VRT.Downloaders.Common.Collections;

namespace VRT.Downloaders.Infrastructure.DownloadExecutor;

public sealed class RemoteStream : IDisposable
{
    private readonly CompositeDisposable _disposables;
    public RemoteStream(Uri url, FileByteRange range)
    {
        _disposables = new CompositeDisposable();
        Url = url;
        Range = range;
    }

    public Uri Url { get; }
    public FileByteRange Range { get; }

    public void Dispose()
    {
        _disposables.Dispose();
    }

    public async Task<Stream> Open()
    {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
        var req = (HttpWebRequest)WebRequest.Create(Url);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        req.AddRange(Range.From, Range.To);
        var response = await req.GetResponseAsync().DisposeWith(_disposables);
        return response.GetResponseStream().SetDisposable(_disposables);
    }
}
