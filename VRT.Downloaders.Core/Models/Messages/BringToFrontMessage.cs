namespace VRT.Downloaders.Models.Messages
{
    public sealed class BringToFrontMessage
    {
        public BringToFrontMessage(string windowName)
        {
            WindowName = windowName;
        }
        public string WindowName { get; }
    }
}
