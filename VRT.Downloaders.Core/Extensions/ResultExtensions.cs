using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;

namespace VRT.Downloaders
{
    public static class ResultExtensions
    {
        public static async Task<Result> BindWithRetry(this Task<Result> result, Func<Task<Result>> asyncAction,
            int numberOfRetries = 3, int delayBetweenRetriesMs = 500)
        {
            Guard.AgainstNull(asyncAction, nameof(asyncAction));

            var currentState = await result;
            if (currentState.IsFailure)
                return currentState;

            var cnt = 0;
            Result lastResult;
            do
            {
                lastResult = await asyncAction().ConfigureAwait(false);
                if (lastResult.IsSuccess)
                    break;
                await Task.Delay(delayBetweenRetriesMs);
                cnt++;
            } while (cnt < numberOfRetries);
            return lastResult;
        }
    }
}
