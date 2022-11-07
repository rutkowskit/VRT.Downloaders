namespace VRT.Downloaders.Extensions;
public static class StringExtensions
{
    public static Result<string> SuccessIfNotEmpty(this string text)
    {
        return string.IsNullOrWhiteSpace(text)
            ? Result.Failure<string>("text is empty")
            : text;
    }
}
