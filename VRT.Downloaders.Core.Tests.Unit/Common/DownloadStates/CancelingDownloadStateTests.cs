using VRT.Downloaders.Fakes;
using Xunit;

namespace VRT.Downloaders.Common.DownloadStates;

public sealed class CancelingDownloadStateTests : BaseTestDownloadState<CancelingDownloadState>
{
    private const string _cancelReason = "TestCanceling";
    [Fact]
    public void Cancel_InThisState_ShouldFail()
    {
        CreateContextWithSut()
            .AssertCancel(false, "Cannot cancel when cancellation is in progress")
            .AssertCurrentStateType<CancelingDownloadState>();

    }

    [Fact]
    public void Remove_InThisState_ShouldFail()
    {
        CreateContextWithSut()
            .AssertRemove(false, "Cannot remove task when cancellation is in progress")
            .AssertCurrentStateType<CancelingDownloadState>();
    }

    [Fact]
    public async Task Download_InThisState_ShouldFail()
    {
        await CreateContextWithSut()
            .AssertDownload(false, "Cannot download task when cancellation is in progress")
            .AssertCurrentStateType<CancelingDownloadState>();
    }

    [Fact]
    public void Transition_ToThisState_ShouldSetCorrectReasonInContext()
    {
        CreateContextWithSut()
            .AssertLastError(_cancelReason);
    }

    private protected override CancelingDownloadState CreateSut()
    {
        return new CancelingDownloadState(_cancelReason);
    }
}