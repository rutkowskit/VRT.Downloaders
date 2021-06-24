using System.Threading.Tasks;
using Xunit;

namespace VRT.Downloaders.Services.Downloads.DownloadStates
{
    public sealed class FinishedDownloadStateTests : BaseTestDownloadState<FinishedDownloadState>
    {
        private const string _finishReason = "TestFinished";
        [Fact]
        public void Cancel_InThisState_ShouldFail()
        {
            CreateContextWithSut()
                .AssertCancel(false, "Cannot cancel finished task")
                .AssertCurrentStateType<FinishedDownloadState>()
                .AssertLastError(_finishReason);
        }

        [Fact]
        public void Remove_InThisState_ShouldSucceed()
        {
            CreateContextWithSut()
                .AssertRemove(true, "Removing finished tasks should be possible")
                .AssertCurrentStateType<RemovedDownloadState>()
                .AssertLastError("");
        }

        [Fact]
        public async Task Download_InThisState_ShouldFail()
        {
            await CreateContextWithSut()
                .AssertDownload(false, "Cannot download finished task")            
                .AssertCurrentStateType<FinishedDownloadState>()
                .AssertLastError(_finishReason);
        }

        private protected override FinishedDownloadState CreateSut()
        {
            return new FinishedDownloadState(_finishReason);
        }
    }
}