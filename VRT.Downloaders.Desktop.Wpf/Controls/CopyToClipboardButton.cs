using System.Windows;
using System.Windows.Controls;

namespace VRT.Downloaders.Desktop.Wpf.Controls
{
    public class CopyToClipboardButton : Button
    {
        public static readonly DependencyProperty ClipboardDataSourceProperty
           = DependencyProperty.Register(nameof(ClipboardDataSource), typeof(object), typeof(CopyToClipboardButton));

        public CopyToClipboardButton()
        {
            ToolTip = "Copy to clipboard";
        }
        public object ClipboardDataSource
        {
            get => GetValue(ClipboardDataSourceProperty);
            set => SetValue(ClipboardDataSourceProperty, value);
        }
        protected override void OnClick()
        {
            if (ClipboardDataSource != null)
            {
                Clipboard.SetDataObject(ClipboardDataSource);
            }
        }
    }
}
