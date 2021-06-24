using CSharpFunctionalExtensions;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace VRT.Downloaders.Services.Downloads.DownloadStates
{
    public sealed class DownloadingDownloadStateTests : BaseTestDownloadState<DownloadingDownloadState>
    {
        private readonly FakeDownloadExecutor _fakeDownloadExecutor;

        public DownloadingDownloadStateTests()
        {
            _fakeDownloadExecutor = new FakeDownloadExecutor();
        }

        [Fact]
        public void Cancel_InThisState_ShouldSucced()
        {
            CreateContextWithSut()
                .AssertCancel(true, "Cancellation in this state should be available")
                .AssertCurrentStateType<CancelingDownloadState>();
        }

        [Fact]
        public void Cancel_WithSimulatedFailure_ShouldFailAndStayInTheSameState()
        {
            const string simulatedFailureMessage = "InternalFailure";

            _fakeDownloadExecutor
                .WithSimulatedResult(Result.Failure(simulatedFailureMessage));

            var context = CreateContextWithSut();
            context.CanCancel.Should()
                .Be(true, "Cancellation in this state should be available");

            var result = context.CurrentState.Cancel(context);
            
            result.IsSuccess.Should().Be(false, "Fake Download Executor returned simulated failure");
            result.Error.Should().Be(simulatedFailureMessage);
            context.AssertCurrentStateType<DownloadingDownloadState>();
        }

        [Fact]
        public void Remove_InThisState_ShouldFail()
        {
            CreateContextWithSut()
                .AssertRemove(false, "Cannot remove task when Canceling is in progress")
                .AssertCurrentStateType<DownloadingDownloadState>();
        }

        [Fact]
        public async Task Download_InThisState_ShouldSetStateToFinished()
        {
            await CreateContextWithSut()
                .AssertDownload(true, "Downloading should be available in this state")
                .AssertCurrentStateType<FinishedDownloadState>();
        }

        [Fact]
        public async Task Download_WithSimulatedFailure_ShouldFailAndStayInTheSameState()
        {
            const string simulatedFailureMessage = "DownloadFailure";
            _fakeDownloadExecutor
                .WithSimulatedResult(Result.Failure(simulatedFailureMessage));

            var context = CreateContextWithSut();

            var result = await context.CurrentState.Download(context);
            
            result.IsSuccess.Should().Be(false, "Download executor returns simulated failure");
            result.Error.Should().Be(simulatedFailureMessage);
            context.AssertCurrentStateType<FinishedDownloadState>();
        }

        private protected override DownloadingDownloadState CreateSut()
        {
            return new DownloadingDownloadState(_fakeDownloadExecutor);
        }
    }
}