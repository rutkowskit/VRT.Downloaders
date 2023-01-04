using VRT.Downloaders.Properties;
using Xunit;

namespace VRT.Downloaders.Common.DownloadStates;

public abstract class BaseTestDownloadState<TDownloadState>
    where TDownloadState : BaseDownloadState
{
    [Fact()]
    public void TransitionToState_InTestContext_ShouldSetCorrectContextProperties()
    {
        var context = CreateContextWithSut();

        context.CurrentState.Should()
            .NotBeNull("State should be set")
            .And.BeOfType<TDownloadState>();
        context.State.Should().Be(context.CurrentState!.State, "Transition should set context state");
        context.CanCancel.Should().Be(context.CurrentState.CanCancel, "Transition should set correct CanCancel flag");
        context.CanRemove.Should().Be(context.CurrentState.CanRemove, "Transition should set correct CanRemove flag");
        context.Request.Should().BeNull("No test context request is available by default");
        context.Partitions.Should().BeNull("No partitions should be present by deufalt");
    }

    [Fact()]
    public void TransitionToState_SameAsCurrentState_ShouldFail()
    {
        var context = CreateContextWithSut();
        var prevState = context.CurrentState;

        context.TransitionToState(context.CurrentState!);

        context.AssertLastError(Resources.Error_StateAlreadySet_StateName.Format(context.State));
        prevState!.State.Should().Be(context.State);
    }

    private protected virtual FakeDownloadContext CreateContextWithSut()
    {
        var result = new FakeDownloadContext();
        result.TransitionToState(CreateSut());
        return result;
    }
    private protected abstract TDownloadState CreateSut();
}