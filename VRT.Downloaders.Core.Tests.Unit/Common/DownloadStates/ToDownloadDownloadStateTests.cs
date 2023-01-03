using VRT.Downloaders.Fakes;
using Xunit;

namespace VRT.Downloaders.Common.DownloadStates;

public sealed class ToDownloadDownloadStateTests : BaseTestMarkerDownloadState<ToDownloadDownloadState>
{
    [Fact]
    public void Remove_InThisState_ShouldSucceed()
    {
        CreateContextWithSut()
            .AssertRemove(true, "Removing task 'to download' should be possible")
            .AssertCurrentStateType<RemovedDownloadState>();
    }
}