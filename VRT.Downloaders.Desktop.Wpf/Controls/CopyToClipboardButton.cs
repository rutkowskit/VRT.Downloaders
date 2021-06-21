using ReactiveUI;
using System.Windows;
using System.Windows.Controls;
using VRT.Downloaders.Models.Messages;

namespace VRT.Downloaders.Desktop.Wpf.Controls
{
    public class CopyToClipboardButton : Button
    {
        public static readonly DependencyProperty ClipboardDataSourceProperty
           = DependencyProperty.Register(nameof(ClipboardDataSource), typeof(object), typeof(CopyToClipboardButton));

        public static readonly DependencyProperty MessageBusProperty
           = DependencyProperty.Register(nameof(MessageBus), typeof(IMessageBus), typeof(CopyToClipboardButton));

        public CopyToClipboardButton()
        {
            ToolTip = "Copy to clipboard";
        }
        public object ClipboardDataSource
        {
            get => GetValue(ClipboardDataSourceProperty);
            set => SetValue(ClipboardDataSourceProperty, value);
        }

        public IMessageBus MessageBus
        {
            get => (IMessageBus)GetValue(MessageBusProperty);
            set => SetValue(MessageBusProperty, value);
        }
        protected override void OnClick()
        {
            if (ClipboardDataSource != null)
            {
                Clipboard.SetDataObject(ClipboardDataSource);
                MessageBus?.SendMessage(new NotifyMessage("Success", "Link copied to clipboard"));
            }
        }
    }
}
