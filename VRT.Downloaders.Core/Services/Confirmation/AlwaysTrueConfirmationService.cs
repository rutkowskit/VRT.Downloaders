namespace VRT.Downloaders.Services.Confirmation;
internal sealed class AlwaysTrueConfirmationService : IConfirmationService
{
    public Task<bool> Confirm(string message, string title)
    {
        return Task.FromResult(true);
    }
}
