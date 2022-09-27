namespace VRT.Downloaders.Services.Confirmation;

public interface IConfirmationService
{
    Task<bool> Confirm(string message, string title);
}
