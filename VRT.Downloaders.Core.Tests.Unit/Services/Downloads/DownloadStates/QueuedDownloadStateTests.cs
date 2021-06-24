using System.Threading.Tasks;
using VRT.Downloaders.Properties;
using Xunit;

namespace VRT.Downloaders.Services.Downloads.DownloadStates
{
    public sealed class QueuedDownloadStateTests : BaseTestDownloadState<QueuedDownloadState>
    {
        private readonly FakeDownloadExecutor _fakeDownloadExecutor;

        public QueuedDownloadStateTests()
        {
            _fakeDownloadExecutor = new FakeDownloadExecutor();
        }

        [Fact]
        public void Cancel_InThisState_ShouldFail()
        {
            CreateContextWithSut()
                .AssertCancel(false, "Cannot cancel queued task")                
                .AssertCurrentStateType<QueuedDownloadState>()
                .AssertLastError("");
        }

        [Fact]
        public void Remove_InThisState_ShouldSucceed()
        {
            CreateContextWithSut()
                .AssertRemove(true, "Removing queued task should be possible")
                .AssertCurrentStateType<RemovedDownloadState>();
        }

        [Fact]
        public async Task Download_InThisState_ShouldSucceed()
        {
            await CreateContextWithSut()
                .AssertDownload(true, "Download should succeed for queued task")
                .AssertLastError(Resources.Msg_Success);
        }

        private protected override QueuedDownloadState CreateSut()
        {
            return new QueuedDownloadState(_fakeDownloadExecutor);
        }
    }
}