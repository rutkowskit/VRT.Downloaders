using CSharpFunctionalExtensions;

namespace VRT.Downloaders
{
    public static class ResultExtensions
    {
        public static Result<T> ToSuccess<T>(this T data)
        {
            return Result.Success(data);
        }
    }
}
