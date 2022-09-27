using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace VRT.Downloaders.Services.Downloads
{
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
            var req = (HttpWebRequest)WebRequest.Create(Url);
            req.AddRange(Range.From, Range.To);
            var response = await req.GetResponseAsync().DisposeWith(_disposables);
            return response.GetResponseStream().SetDisposable(_disposables);
        }
    }
}
