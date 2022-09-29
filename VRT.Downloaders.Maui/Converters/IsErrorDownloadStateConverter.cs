using CommunityToolkit.Maui.Converters;
using System.Globalization;
using VRT.Downloaders.Services.Downloads.DownloadStates;

namespace VRT.Downloaders.Maui.Converters;
public sealed class IsErrorDownloadStateConverter
    : BaseConverter<BaseDownloadState.States, bool>
{
    public override bool DefaultConvertReturnValue { get; set; }
    public override BaseDownloadState.States DefaultConvertBackReturnValue { get; set; }

    public override BaseDownloadState.States ConvertBackTo(bool value, CultureInfo culture)
    {
        return value
            ? BaseDownloadState.States.Error
            : DefaultConvertBackReturnValue;
    }

    public override bool ConvertFrom(BaseDownloadState.States value, CultureInfo culture)
    {
        return value == BaseDownloadState.States.Error;
    }
}
