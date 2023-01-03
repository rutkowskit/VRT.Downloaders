using VRT.Downloaders.Fakes;
using Xunit;

namespace VRT.Downloaders.Common.DownloadStates;

public sealed class RemovedDownloadStateTests : BaseTestMarkerDownloadState<RemovedDownloadState>
{
    [Fact]
    public void Remove_InThisState_ShouldFail()
    {
        CreateContextWithSut()
            .AssertRemove(false, "Removing already removed tasks should not be possible")
            .AssertCurrentStateType<RemovedDownloadState>();
    }
}