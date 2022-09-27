namespace VRT.Downloaders.Models.Messages;
public sealed record BringToFrontMessage(string WindowName) : MediatR.INotification;