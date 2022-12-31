using CSharpFunctionalExtensions;

namespace VRT.Downloaders;

public static class TaskListExtensions
{
    public static Task<Result<IReadOnlyCollection<TResponse>>> DoParallel<TResponse>(this IList<Task<Result<TResponse>>> tasks,
        Action<TResponse>? onSuccess = null, bool continueOnFailure = false, Action<Result>? onFailure = null,
        CancellationToken cancellationToken = default)
    {
        var onSuccessAsync = new Func<TResponse, Task>((r) => { onSuccess?.Invoke(r); return Task.CompletedTask; });
        var onFailureAsync = new Func<Result, Task>((r) => { onFailure?.Invoke(r); return Task.CompletedTask; });
        return tasks.DoParallel(onSuccessAsync, continueOnFailure, onFailureAsync, cancellationToken);
    }
    public static Task<Result<IReadOnlyCollection<TResponse>>> DoParallelAsync<TResponse>(this IList<Task<Result<TResponse>>> tasks,
        Func<TResponse, Task>? onSuccessAsync = null, bool continueOnFailure = false, Func<Result, Task>? onFailureAsync = null,
        CancellationToken cancellationToken = default)
    {
        return tasks.DoParallel(onSuccessAsync, continueOnFailure, onFailureAsync, cancellationToken);
    }

    public static async Task<Result<IReadOnlyCollection<TResponse>>> DoParallel<TResponse>(this IList<Task<Result<TResponse>>> tasks,
        Func<TResponse, Task>? onSuccess, bool continueOnFailure, Func<Result, Task>? onFailure,
        CancellationToken cancellationToken)

    {
        var result = new List<Result<TResponse>>();
        while (tasks.Count > 0)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Result.Failure<IReadOnlyCollection<TResponse>>("Operation canceled");
            }
            var finished = await PopAnyFinishedTask(tasks)
                .TapIf(onSuccess is not null, t => onSuccess!(t));
            result.Add(finished);

            if (finished.IsSuccess)
                continue;
            if (onFailure != null)
            {
                await onFailure(finished);
            }
            if (continueOnFailure == false)
                return Result.Failure<IReadOnlyCollection<TResponse>>(finished.Error);
        }
        return result
            .Where(x => x.IsSuccess)
            .Select(x => x.Value)
            .AsReadOnlyCollection()
            .ToSuccess();
    }

    private static async Task<Result<T>> PopAnyFinishedTask<T>(IList<Task<Result<T>>> tasks)
    {
        try
        {
            var finishedTask = await Task.WhenAny(tasks);
            tasks.Remove(finishedTask);
            var taskResult = await finishedTask;
            return taskResult;
        }
        catch (TaskCanceledException ex)
        {
            return Result.Failure<T>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<T>(ex.ToString());
        }
    }
}