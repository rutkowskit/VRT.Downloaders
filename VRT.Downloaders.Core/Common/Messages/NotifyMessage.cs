namespace VRT.Downloaders.Common.Messages;

public sealed record NotifyMessage(string Type, string Message) : MediatR.INotification;
