using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using VRT.Downloaders.Models.Messages;
using VRT.Downloaders.Properties;
using VRT.Downloaders.Services.Configs;
using VRT.Downloaders.Services.Downloads;
using VRT.Downloaders.Services.Downloads.DownloadStates;
using VRT.Downloaders.Services.Medias;

namespace VRT.Downloaders.ViewModels
{
    public sealed class MainWindowViewModel : BaseViewModel
    {
        private readonly IDownloadQueueService _downloadService;
        private readonly IMediaService _mediaService;        
        private readonly ReadOnlyObservableCollection<DownloadTaskProxy> _downloads;
        private readonly SourceList<MediaInfo> _medias;

        public MainWindowViewModel(IDownloadQueueService downloadService,
            IMediaService mediaService,
            IMessageBus messageBus,
            IAppSettingsService settings)
        {
            Title = Resources.Title_Downloads;
            ServicePointManager.DefaultConnectionLimit = 1000;            
            GetMediasCommand = ReactiveCommand.CreateFromTask(GetMedias, CanGetMedias());
            DownloadMediaCommand = ReactiveCommand.CreateFromTask<MediaInfo>(DownloadMedia);
            ProcesUrlCommand = ReactiveCommand.Create<string>(ProcessUrl);
            ClearFinishedCommand = ReactiveCommand.Create(ClearFinished);

            Medias = new ObservableCollectionExtended<MediaInfo>();
            _medias = new SourceList<MediaInfo>();
            _medias.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(Medias)
                .Subscribe()
                .DisposeWith(Disposables)
                .Discard();

            _downloadService = downloadService;
            _mediaService = mediaService;
            SettingsService = settings;
            MessageBus = messageBus;            

            _downloadService.LiveDownloads.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(t => new DownloadTaskProxy(t))
                .Bind(out _downloads)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(Disposables)
                .Discard();
        }

        public ReadOnlyObservableCollection<DownloadTaskProxy> Downloads => _downloads;

        [Reactive] public string Uri { get; set; }        
        [Reactive] public IMessageBus MessageBus { get; private set; }
        public IAppSettingsService SettingsService { get; }
        public IObservableCollection<MediaInfo> Medias { get; set; }

        public ICommand DownloadMediaCommand { get; }
        public ICommand GetMediasCommand { get; }
        public ICommand ProcesUrlCommand { get; }
        public ICommand ClearFinishedCommand { get; }

        private Task DownloadMedia(MediaInfo media)
        {
            var outputDir = SettingsService.GetSettings().OutputDirectory;
            var request = media.ToDownloadRequest(outputDir);
            return _downloadService.AddDownloadTask(request).Discard();
        }

        private async Task GetMedias()
        {
            _medias.Clear();
            var getMediaResult = await Result.Success()
                .OnSuccessTry(() => _mediaService.GetAvailableMedias(Uri)
                    .OnFailureCompensate(_ => CreateDirectDownload(Uri)))
                .Bind(medias => medias)
                .Tap(medias => medias.SetDefaultOutputFileName());

            // call it after await to avoid Dispatcher problems when updating fields connected with binded properties
            getMediaResult
                .Tap(medias => _medias.AddRange(medias))
                .OnFailure(error => MessageBus.SendMessage(new NotifyMessage("Error", error)))
                .Discard();
        }

        private IObservable<bool> CanGetMedias()
        {
            return this.WhenValueChanged(p => p.Uri)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(uri => !string.IsNullOrWhiteSpace(uri));
        }

        private void ClearFinished()
        {
            Downloads
                .Where(download => download.State == BaseDownloadState.States.Finished)
                .ToList()
                .ForEach(download => download.RemoveTaskCommand.Execute(Unit.Default));
        }

        private static async Task<Result<MediaInfo[]>> CreateDirectDownload(string uri)
        {
            if (!System.Uri.TryCreate(uri, UriKind.Absolute, out var resourceUri))
            {
                return Result.Failure<MediaInfo[]>("Invalid Url");
            }

            await Task.Yield();
            var extension = GetMediaExtension(uri);
            var mediaInfo = new MediaInfo()
            {
                Uri = resourceUri,
                FormatDescription = string.IsNullOrWhiteSpace(extension)
                        ? "unknown"
                        : extension,
                Extension = extension,
                Title = HttpUtility.UrlDecode(Path.GetFileName(uri))
            };                        
            return new[] { mediaInfo };
        }

        private static string GetMediaExtension(string uri)
        {
            var extension = Path.GetExtension(uri);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = "unknown";
            }
            return extension;
        }

        private void ProcessUrl(string text)
        {
            if (!System.Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out var result)
                || !result.IsAbsoluteUri)
            {
                return;
            }

            if (result.AbsolutePath != null && result.AbsolutePath.Contains("watch"))
            {
                var prevUri = Uri;
                Uri = result.AbsoluteUri;
                if (prevUri != Uri)
                {
                    MessageBus.SendMessage(new BringToFrontMessage("Main"));
                }
            }
        }
    }
}
