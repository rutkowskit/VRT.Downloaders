using MediatR;
using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Extensions;
using VRT.Downloaders.Services.Medias;

namespace VRT.Downloaders.Medias.Commands.QueueDownloadTask;
public sealed record QueueDownloadTaskCommand(MediaInfo Media) : IRequest<Result>
{
    internal sealed class QueueDownloadTaskCommandHandler : IRequestHandler<QueueDownloadTaskCommand, Result>
    {
        private readonly IAppSettingsService _settingsService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IConfirmationService _confirmationService;
        private readonly IDownloadQueueService _downloadService;

        public QueueDownloadTaskCommandHandler(
            IAppSettingsService settingsService,
            IFileSystemService fileSystemService,
            IConfirmationService confirmationService,
            IDownloadQueueService downloadService)
        {
            _settingsService = settingsService;
            _fileSystemService = fileSystemService;
            _confirmationService = confirmationService;
            _downloadService = downloadService;
        }
        public async Task<Result> Handle(QueueDownloadTaskCommand request, CancellationToken cancellationToken)
        {
            var outputDir = _settingsService.GetSettings().OutputDirectory;
            var downloadRequest = request.Media.ToDownloadRequest(outputDir);
            var fullOutpuFile = Path.Combine(outputDir, downloadRequest.OutputFileName);
            if (_fileSystemService.FileExists(fullOutpuFile))
            {
                var overwriteConfirmed = await _confirmationService.Confirm("Output file already exists. Are you sure you want to overwrite the file ?", "Confirm overwrite");
                if (overwriteConfirmed is false)
                {
                    return Result.Failure("Add task cancelled");
                }
            }
            var result = await _downloadService.AddDownloadTask(downloadRequest);
            return result;
        }
    }
}
