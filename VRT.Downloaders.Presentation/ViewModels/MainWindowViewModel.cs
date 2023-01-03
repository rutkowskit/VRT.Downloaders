﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Reactive.Concurrency;
using VRT.Downloaders.Presentation.Extensions;

namespace VRT.Downloaders.Presentation.ViewModels;

public sealed partial class MainWindowViewModel : BaseViewModel
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
            .DisposeWith(Disposables);

        this.WhenAnyValue(p => p.MediaToAutoDownload)
            .Where(p => p is not null)
            .Throttle(TimeSpan.FromMilliseconds(600))
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(DownloadMediaCommand)
            .DisposeWith(Disposables);
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadMediaCommand))]
    [NotifyCanExecuteChangedFor(nameof(GetMediasCommand))]
    private bool _isRefreshing;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadMediaCommand))]
    [NotifyCanExecuteChangedFor(nameof(GetMediasCommand))]
    private string? _uri;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadMediaCommand))]
    private MediaInfo? _mediaToAutoDownload;

    public ReadOnlyObservableCollection<DownloadTaskProxy> Downloads => _downloads;
    public IAppSettingsService SettingsService { get; }
    public IObservableCollection<MediaInfo> Medias { get; set; }

    [RelayCommand]
    private async Task ShowDownloadError(DownloadTaskProxy task)
    {
        var request = new ShowErrorRequest(task.LastErrorMessage ?? Resources.Error_UnknownError);
        await _mediator.Send(request);
    }

    [RelayCommand()]
    private async Task DownloadMedia(MediaInfo media)
    {
        var outputDir = SettingsService.GetSettings().OutputDirectory;
        var request = media.ToDownloadRequest(outputDir);
        await _downloadService.AddDownloadTask(request).Discard();
    }

    [RelayCommand(CanExecute = nameof(CanGetMedias))]
    private async Task GetMedias()
    {
        _medias.Clear();

        try
        {            
            this.DoOnDispatcher(vm => vm.IsRefreshing = true);
            var uri = Uri!;
            var getMediaResult = await GetMediaService(uri)
                .BindTry(mediaService => mediaService.GetAvailableMedias(uri))
                .Tap(medias => medias.SetDefaultOutputFileName());

            // call it after await to avoid Dispatcher problems when updating fields connected with binded properties
            await getMediaResult
                .Tap(medias => _medias.AddRange(medias))
                .TapError(error => _mediator.Publish(new NotifyMessage("Error", error)))
                .Bind(GetMediaToAutoDownload)
                .Tap(media => MediaToAutoDownload = media); ;
        }
        finally
        {
            this.DoOnDispatcher(vm => vm.IsRefreshing = false);            
        }
    }
    private bool CanGetMedias() => IsRefreshing is false;

    [RelayCommand]
    private void ClearFinished()
    {
        Downloads
            .Where(download => download.State == BaseDownloadState.States.Finished)
            .ToList()
            .ForEach(download => download.RemoveTaskCommand.Execute(System.Reactive.Unit.Default));
    }

    [RelayCommand(CanExecute = nameof(CanProcessUrl))]
    private async Task ProcessUrl(string text)
    {
        var result = await TryCreateResourceUrl(text)
            .Bind(CheckIfMediaServiceExists)
            .Bind(CheckIfMediaUrlHasChanged);
        await result
            .Tap(url =>
            {
                Uri = url.AbsoluteUri;
                _mediator.Publish(new BringToFrontMessage("Main"));
            })
            .TapIf(ShouldAutoRefreshMediaList(), GetMedias);
    }
    private bool CanProcessUrl(string text) => string.IsNullOrWhiteSpace(text) is false;

    private Result<MediaInfo> GetMediaToAutoDownload(IReadOnlyCollection<MediaInfo> medias)
    {
        var result = Result.Success(SettingsService.GetSettings())
            .Bind(s => s.AutoDownloadMediaTypePattern.NotEmpty())
            .Bind(medias.FindFirstByDescription);
        return result;
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
    

    private bool ShouldAutoRefreshMediaList()
    {
        return SettingsService.GetSettings().EnableAutoGetMedias
            && string.IsNullOrEmpty(Uri) is false;
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