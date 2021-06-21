namespace VRT.Downloaders.Models.Messages
{
    public sealed class NotifyMessage
    {
        public NotifyMessage(string type, string message)
        {
            Type = type;
            Message = message;
        }
        public string Type { get; }
        public string Message { get; }
    }
}
