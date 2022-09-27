namespace VRT.Downloaders.Models.Messages;

public sealed record NotifyMessage(string Type, string Message) : MediatR.INotification;
