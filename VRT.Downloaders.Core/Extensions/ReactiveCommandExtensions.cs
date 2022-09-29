using MediatR;

namespace VRT.Downloaders.Extensions;

public static class ReactiveCommandExtensions
{
    public static ReactiveCommandBase<TParam, TResult> WithExceptionHandler<TParam, TResult>(
       this ReactiveCommandBase<TParam, TResult> command,
       Func<Exception, Task> exceptionHandler)
    {
        command.ThrownExceptions.Subscribe(async e => await exceptionHandler(e));
        return command;
    }

    public static ReactiveCommandBase<TParam, TResult> WithNotifyExceptionHandler<TParam, TResult>(
       this ReactiveCommandBase<TParam, TResult> command, IMediator mediator)
    {
        command.ThrownExceptions.Subscribe(async e => await HandleException(e, mediator));
        return command;
    }
    public static ReactiveCommandBase<TParam, TResult> WithDevNullExceptionHandler<TParam, TResult>(
       this ReactiveCommandBase<TParam, TResult> command)
    {
        command.ThrownExceptions.Subscribe(); // do nothing with exception
        return command;
    }


    private static async Task HandleException(Exception ex, IMediator mediator)
    {
        var message = new NotifyMessage("Error", ex.Message);
        await mediator.Publish(message);
    }
}
