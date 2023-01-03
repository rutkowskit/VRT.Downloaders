namespace VRT.Downloaders.Common.Abstractions;

public interface IConfirmationService
{
    Task<bool> Confirm(string message, string title);
}
