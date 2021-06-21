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
        private readonly IAppSettingsService _settings;
        private readonly ReadOnlyObservableCollection<DownloadTaskProxy> _downloads;
        private readonly SourceList<MediaInfo> _medias;

        public MainWindowViewModel(IDownloadQueueService downloadService,
            IMediaService mediaService,
            IMessageBus messageBus,
            IAppSettingsService settings)
        {
            ServicePointManager.DefaultConnectionLimit = 1000;
            Title = "Downloads";
            GetMediasCommand = ReactiveCommand.CreateFromTask(GetMedias, CanGetMedias());
            DownloadMediaCommand = ReactiveCommand.CreateFromTask<MediaInfo>(DownloadMedia);
            ProcessClipboardUrlCommand = ReactiveCommand.Create<string>(ProcessClipboardUrl);
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
            MessageBus = messageBus;
            _settings = settings;

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
        [Reactive] public string OutputDirectory { get; set; }
        [Reactive] public bool EnableClipboardMonitor { get; set; }
        [Reactive] public IMessageBus MessageBus { get; private set; }

        public IObservableCollection<MediaInfo> Medias { get; set; }

        public ICommand DownloadMediaCommand { get; }
        public ICommand GetMediasCommand { get; }
        public ICommand ProcessClipboardUrlCommand { get; }
        public ICommand ClearFinishedCommand { get; }

        public void OnActivation()
        {
            var settings = _settings.GetSettings();
            EnableClipboardMonitor = settings.EnableClipboardMonitor;
            OutputDirectory = settings.OutputDirectory;

            this.WhenAnyValue(p => p.OutputDirectory, p => p.EnableClipboardMonitor)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(w => !string.IsNullOrWhiteSpace(w.Item1))
                .Subscribe(s => UpdateSettings(s))
                .DisposeWith(Disposables)
                .Discard();
        }

        private Task DownloadMedia(MediaInfo media)
        {
            var request = media.ToDownloadRequest(OutputDirectory);
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

        private void UpdateSettings((string outDir, bool enableMonitor) settings)
        {
            var newSettings = new AppSettings()
            {
                EnableClipboardMonitor = settings.enableMonitor,
                OutputDirectory = settings.outDir
            };
            _settings.SaveSettings(newSettings);
        }

        private static async Task<Result<MediaInfo[]>> CreateDirectDownload(string uri)
        {
            if (!System.Uri.TryCreate(uri, UriKind.Absolute, out var resourceUri))
            {
                return Result.Failure<MediaInfo[]>("Invalid Url");
            }

            await Task.Yield();
            var extension = GetMediaExtension(uri);

            var result = new[]
            {
                new MediaInfo()
                {
                    Uri = resourceUri,
                    FormatDescription = string.IsNullOrWhiteSpace(extension)
                        ? "unknown"
                        : extension,
                    Extension = extension,
                    Title = HttpUtility.UrlDecode(Path.GetFileName(uri))
                }
            };
            return result;
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

        private void ProcessClipboardUrl(string text)
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
