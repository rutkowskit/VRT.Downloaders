using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VRT.Downloaders
{
    public static class TaskListExtensions
    {
        public static async Task<Result> DoParallel<TResponse>(this IList<Task<Result<TResponse>>> tasks,
            Action<TResponse> onSuccess = null, bool continueOnFailure = false)
        {
            var result = new List<Result>();
            while (tasks.Count > 0)
            {
                var finished = await PopAnyFinishedTask(tasks)
                    .Tap(t => onSuccess?.Invoke(t));

                result.Add(finished);

                if (finished.IsSuccess)
                    continue;
                
                if (continueOnFailure == false)
                    return finished;
            }
            return Result.Combine(result);
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
            catch (Exception ex)
            {
                return Result.Failure<T>(ex.ToString());
            }
        }
    }
}
