using MediatR;

namespace VRT.Downloaders.ViewModels;

public sealed class MainWindowViewModel : BaseViewModel
{
    private readonly IDownloadQueueService _downloadService;
    private readonly IMediator _mediator;
    private readonly IMediaService[] _mediaServices;
    private readonly ReadOnlyObservableCollection<DownloadTaskProxy> _downloads;
    private readonly SourceList<MediaInfo> _medias;

    public MainWindowViewModel(IDownloadQueueService downloadService,
        IEnumerable<IMediaService> mediaService,        
        IAppSettingsService settings,
        IMediator mediator)
    {
        Title = Resources.Title_Downloads;
        _medias = new SourceList<MediaInfo>();
        ServicePointManager.DefaultConnectionLimit = 1000;
        GetMediasCommand = ReactiveCommand.CreateFromTask(GetMedias, CanGetMedias());
        DownloadMediaCommand = ReactiveCommand.CreateFromTask<MediaInfo>(DownloadMedia);
        ProcesUrlCommand = ReactiveCommand.CreateFromTask<string>(ProcessUrl);
        ClearFinishedCommand = ReactiveCommand.Create(ClearFinished);

        Medias = new ObservableCollectionExtended<MediaInfo>();
        
        _medias.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(Medias)
            .Subscribe()
            .DisposeWith(Disposables)
            .Discard();

        _downloadService = downloadService;
        _mediaServices = mediaService?.ToArray() ?? Array.Empty<IMediaService>();
        SettingsService = settings;
        _mediator = mediator;

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

    [Reactive] public bool IsRefreshing { get; set; }
    [Reactive] public string Uri { get; set; }    
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

        try
        {
            IsRefreshing = true;
            var getMediaResult = await GetMediaService(Uri)
                .BindTry(mediaService => mediaService.GetAvailableMedias(Uri))
                .Tap(medias => medias.SetDefaultOutputFileName());

            // call it after await to avoid Dispatcher problems when updating fields connected with binded properties
            await getMediaResult
                .Tap(medias => _medias.AddRange(medias))
                .OnFailure(error => _mediator.Publish(new NotifyMessage("Error", error)));
        }
        finally
        {
            IsRefreshing = false;
        }

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
        return this.WhenValueChanged(p => p.IsRefreshing)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(isRefreshing => isRefreshing is false);
    }
    private void ClearFinished()
    {
        Downloads
            .Where(download => download.State == BaseDownloadState.States.Finished)
            .ToList()
            .ForEach(download => download.RemoveTaskCommand.Execute(System.Reactive.Unit.Default));
    }
    private async Task ProcessUrl(string text)
    {
        var result = await TryCreateResourceUrl(text)
            .Bind(CheckIfMediaServiceExists)
            .Bind(CheckIfMediaUrlHasChanged)
            .Tap(url =>
            {
                Uri = url.AbsoluteUri;
                _mediator.Publish(new BringToFrontMessage("Main"));
            });
    }

    private Result<Uri> CheckIfMediaUrlHasChanged(Uri url)
    {
        return url.AbsoluteUri != Uri ? url : Result.Failure<Uri>("Url is the same");
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
