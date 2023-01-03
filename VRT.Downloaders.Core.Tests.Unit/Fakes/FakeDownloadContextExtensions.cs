using VRT.Downloaders.Common.DownloadStates;

namespace VRT.Downloaders.Fakes;

internal static class FakeDownloadContextExtensions
{
    internal static FakeDownloadContext AssertCancel(this FakeDownloadContext context,
        bool shouldBeAvailable, string reason)
    {
        context.CanCancel.Should().Be(shouldBeAvailable, reason);

        context.CurrentState.Cancel(context)
            .IsSuccess.Should().Be(shouldBeAvailable, reason);
        return context;
    }

    internal static FakeDownloadContext AssertRemove(this FakeDownloadContext context,
        bool shouldBeAvailable, string reason)
    {
        context.CanRemove.Should().Be(shouldBeAvailable, reason);

        context.CurrentState.Remove(context)
            .IsSuccess.Should().Be(shouldBeAvailable, reason);
        return context;
    }

    internal static async Task<FakeDownloadContext> AssertDownload(this FakeDownloadContext context,
        bool shouldBeAvailable, string reason)
    {
        var result = await context.CurrentState.Download(context);
        result.IsSuccess.Should().Be(shouldBeAvailable, reason);
        return context;
    }

    internal static FakeDownloadContext AssertLastError(this FakeDownloadContext context,
        string expectedError)
    {
        context.LastErrorMessage.Should().Be(expectedError);
        return context;
    }

    internal static async Task<FakeDownloadContext> AssertLastError(this Task<FakeDownloadContext> context,
        string expectedError)
    {
        var result = await context;
        return result.AssertLastError(expectedError);
    }

    internal static FakeDownloadContext AssertCurrentStateType<TExpectedState>(this FakeDownloadContext context)
        where TExpectedState : BaseDownloadState
    {
        context.CurrentState.Should().BeOfType<TExpectedState>();
        return context;
    }

    internal static async Task<FakeDownloadContext> AssertCurrentStateType<TExpectedState>(this Task<FakeDownloadContext> context)
        where TExpectedState : BaseDownloadState
    {
        var result = await context;
        return result.AssertCurrentStateType<TExpectedState>();
    }
}
