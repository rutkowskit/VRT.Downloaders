using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
        private readonly IMediaService[] _mediaServices;
        private readonly ReadOnlyObservableCollection<DownloadTaskProxy> _downloads;
        private readonly SourceList<MediaInfo> _medias;

        public MainWindowViewModel(IDownloadQueueService downloadService,
            IEnumerable<IMediaService> mediaService,
            IMessageBus messageBus,
            IAppSettingsService settings)
        {
            Title = Resources.Title_Downloads;
            ServicePointManager.DefaultConnectionLimit = 1000;
            GetMediasCommand = ReactiveCommand.CreateFromTask(GetMedias, CanGetMedias());
            DownloadMediaCommand = ReactiveCommand.CreateFromTask<MediaInfo>(DownloadMedia);
            ProcesUrlCommand = ReactiveCommand.CreateFromTask<string>(ProcessUrl);
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
            _mediaServices = mediaService?.ToArray() ?? Array.Empty<IMediaService>();
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

            var getMediaResult = await GetMediaService(Uri)
               .OnSuccessTry(mediaService => mediaService.GetAvailableMedias(Uri))
               .Bind(medias => medias)
               .Tap(medias => medias.SetDefaultOutputFileName());

            // call it after await to avoid Dispatcher problems when updating fields connected with binded properties
            getMediaResult
                .Tap(medias => _medias.AddRange(medias))
                .OnFailure(error => MessageBus.SendMessage(new NotifyMessage("Error", error)))
                .Discard();
        }
        private async Task<Result<IMediaService>> GetMediaService(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return Result.Failure<IMediaService>(Resources.Error_NotSupported);
            }

            foreach (var service in _mediaServices)
            {
                var canGetMedia = await service.CanGetMedia(url);
                if (canGetMedia.IsSuccess)
                    return Result.Success(service);
            }
            return Result.Success<IMediaService>(new DirectMediaService());
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
        private async Task ProcessUrl(string text)
        {
            var result = await TryCreateResourceUrl(text)
                .Bind(CheckIfMediaServiceExists)                
                .Bind(CheckIfMediaUrlHasChanged)
                .Tap(url =>
                {
                    Uri = url.AbsoluteUri;
                    MessageBus.SendMessage(new BringToFrontMessage("Main"));
                });
        }
        
        private Result<Uri> CheckIfMediaUrlHasChanged(Uri url)
        {
            return url.AbsoluteUri != Uri ? url : Result.Failure<Uri>("");
        }
        private async Task<Result<Uri>> CheckIfMediaServiceExists(Uri url)
        {
            return await GetMediaService(url.AbsoluteUri)
                .Bind(s => s.CanGetMedia(url.AbsoluteUri))
                .Map(() => url);
        }

        private static Result<Uri> TryCreateResourceUrl(string text)
        {
            return System.Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out var result)
                    && result.IsAbsoluteUri
                ? result
                : Result.Failure<Uri>(Resources.Error_IncorrectResourceUrl);
        }
    }
}
