using DynamicData.Alias;
using Microsoft.Extensions.Options;
using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Common.Models;
using VRT.Downloaders.Common.Options;
using VRT.Downloaders.Services.DownloadQueue;
using Xunit;

namespace VRT.Downloaders.Workers;

public sealed class DownloadingWorkerTests
{
    private FakeDownloadExecutor DownloadExecutor { get; }
    private DownloadQueueService QueueService { get; }
    private FakeAppStateService AppStateService { get; }
    public DownloadingWorkerTests()
    {
        DownloadExecutor = new FakeDownloadExecutor();
        AppStateService = new FakeAppStateService();
        QueueService = new DownloadQueueService(AppStateService);
    }

    [Fact()]
    public void DownloadingWorker_WhenDownloadQueueServiceIsNull_ShouldThrowArgumentNullException()
    {
        Action act = () => CreateSut(null!, DownloadExecutor);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*downloadQueueService*");

    }
    [Fact()]
    public void DownloadingWorker_WhenDownloadExecutorIsNull_ShouldThrowArgumentNullException()
    {
        Action act = () => CreateSut(QueueService, null!);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*downloadExecutor*");
    }

    [Fact()]
    public void Dispose_WhenDownloadQueueIsEmpty_ShouldDoNothing()
    {
        var sut = CreateSut();
        sut.Dispose();

        sut.IsDisposed.Should().BeTrue();
        sut.DownloadsCount.Should().Be(0);
    }
    [Fact()]
    public async Task DownloadingWorker_WhenDownloadQueueHasOneItem_ShouldProcessDownloadTask()
    {
        var request = new DownloadRequest("Test", new Uri("test://test.txt"), "Test.txt");

        await QueueService.AddDownloadTask(request);

        using var sut = CreateSut();

        List<DownloadTask> tasks = new();
        QueueService.LiveDownloads.Connect().Subscribe(t => tasks = t.Select(x => x.Current).ToList());

        tasks.Should().HaveCount(1);
        var task = tasks[0];
        task.State.Should().Be(Common.DownloadStates.BaseDownloadState.States.Finished);
        task.CanCancel.Should().BeFalse();
        task.CanRemove.Should().BeTrue();
    }

    private DownloadingWorker CreateSut()
    {
        return CreateSut(QueueService, DownloadExecutor);
    }

    private static DownloadingWorker CreateSut(IDownloadQueueService queueService, IDownloadExecutorService downloadExecutor)
    {
        var options = new DownloadingWorkerOptions()
        {
            IdleDelayTimeMilliseconds = 1,
            MaxConcurrentDownloads = 1
        };
        return new DownloadingWorker(queueService, downloadExecutor, Options.Create(options));
    }
}