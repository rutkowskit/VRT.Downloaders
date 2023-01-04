using DynamicData.Alias;
using Microsoft.Extensions.Options;
using VRT.Assets.Application.Common.Abstractions;
using VRT.Downloaders.Common.Abstractions;
using VRT.Downloaders.Common.DownloadStates;
using VRT.Downloaders.Common.Factories;
using VRT.Downloaders.Common.Models;
using VRT.Downloaders.Common.Options;
using VRT.Downloaders.Services.DownloadQueue;
using Xunit;

namespace VRT.Downloaders.Workers;

public sealed class DownloadingWorkerTests
{
    private FakeDownloadExecutor DownloadExecutor { get; }
    private DownloadQueueService QueueService { get; }    
    public DownloadingWorkerTests()
    {
        DownloadExecutor = new FakeDownloadExecutor();
        QueueService = new DownloadQueueService();
    }

    [Fact()]
    public void DownloadingWorker_WhenDownloadQueueServiceIsNull_ShouldThrowArgumentNullException()
    {
        Action act = () => CreateSut(null!, new AbstractFactory<IDownloadExecutorService>(() => DownloadExecutor));

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
        DownloadRequest request = new("Test", new Uri("test://test.txt"), "Test.txt");
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

    [Fact()]
    public async Task DownloadingWorker_WhenDownloadQueueHasThreeItems_ShouldProcessAllDownloads()
    {
        var sut = CreateSut();

        HashSet<DownloadTask> tasks = new();
        QueueService.LiveDownloads.Connect().Subscribe(t => t.Select(x => x.Current).ToList().ForEach(t => tasks.Add(t)));
        DownloadRequest request1 = new("Test", new Uri("test://test.txt"), "Test.txt");
        DownloadRequest request2 = new("Test2", new Uri("test://test2.txt"), "Test2.txt");
        DownloadRequest request3 = new("Test3", new Uri("test://test3.txt"), "Test3.txt");

        DownloadExecutor.WithExecutionDelay(TimeSpan.FromMilliseconds(50), request1.Name);
        await QueueService.AddDownloadTask(request1);
        await QueueService.AddDownloadTask(request2);
        await QueueService.AddDownloadTask(request3);
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        tasks.Should().HaveCount(3);
        tasks.Should().AllSatisfy(t => t.State.Should().Be(BaseDownloadState.States.Finished));
        sut.Dispose();
    }

    [Fact]
    public async Task DownloadingWorker_WhenDownloadTaskFail_ShouldBeInErrorState()
    {
        var expectedResult = Result.Failure("SimulatedError");
        DownloadExecutor.WithSimulatedResult(expectedResult);
        DownloadRequest request1 = new("Test", new Uri("test://test.txt"), "Test.txt");
        var downloadTask = await QueueService.AddDownloadTask(request1);

        using var sut = CreateSut();

        downloadTask.IsSuccess.Should().BeTrue();
        downloadTask.Value.DownloadProgress.Should().Be(0);
        downloadTask.Value.State.Should().Be(BaseDownloadState.States.Error);
        downloadTask.Value.LastErrorMessage.Should().Be(expectedResult.Error);
    }
    [Fact()]
    public async Task DownloadingWorker_WhenDisposeWhileRunningDownloadingTask_DownloadingTaskShouldRemainInDownloadingState()
    {
        DownloadRequest request = new("Test", new Uri("test://test.txt"), "Test.txt");
        DownloadExecutor.WithExecutionDelay(TimeSpan.FromMinutes(1), request.Name);
        var downloadTask = (await QueueService.AddDownloadTask(request)).Value;

        var sut = CreateSut();        
        sut.Dispose();

        downloadTask.State.Should().Be(BaseDownloadState.States.Downloading);
    }

    private DownloadingWorker CreateSut()
    {
        var executorFactory = new AbstractFactory<IDownloadExecutorService>(() => DownloadExecutor);
        return CreateSut(QueueService, executorFactory);
    }

    private static DownloadingWorker CreateSut(IDownloadQueueService queueService,
        IAbstractFactory<IDownloadExecutorService> downloadExecutorFactory)
    {
        var options = new DownloadingWorkerOptions()
        {
            IdleDelayTimeMilliseconds = 1,
            MaxConcurrentDownloads = 1
        };
        return new DownloadingWorker(queueService, downloadExecutorFactory, Options.Create(options));
    }
}