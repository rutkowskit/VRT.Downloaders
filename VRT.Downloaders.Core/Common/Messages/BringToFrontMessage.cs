namespace VRT.Downloaders.Common.Messages;
public sealed record BringToFrontMessage(string WindowName) : MediatR.INotification;