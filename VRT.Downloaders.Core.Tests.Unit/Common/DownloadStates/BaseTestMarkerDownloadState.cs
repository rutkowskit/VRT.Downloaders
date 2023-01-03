using VRT.Downloaders.Fakes;
using Xunit;

namespace VRT.Downloaders.Common.DownloadStates;

/// <summary>
/// Base test for Download States that acts only as markers (they do only state transition without any side effects)
/// </summary>
/// <typeparam name="TDownloadState">Download state</typeparam>
public abstract class BaseTestMarkerDownloadState<TDownloadState> : BaseTestDownloadState<TDownloadState>
    where TDownloadState : BaseDownloadState, new()
{

    [Fact]
    public void Cancel_InThisState_ShouldFail()
    {
        CreateContextWithSut()
            .AssertCancel(false, "Cannot cancel queued task")
            .AssertCurrentStateType<TDownloadState>();
    }

    private protected override TDownloadState CreateSut() => new TDownloadState();
}