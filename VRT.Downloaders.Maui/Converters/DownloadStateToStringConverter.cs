using CommunityToolkit.Maui.Converters;
using System.Globalization;
using VRT.Downloaders.Common.DownloadStates;

namespace VRT.Downloaders.Maui.Converters;
public sealed class DownloadStateToStringConverter
    : BaseConverter<BaseDownloadState.States, string?>
{    
    public override string? DefaultConvertReturnValue { get; set; }
    public override BaseDownloadState.States DefaultConvertBackReturnValue { get; set; }

    public override BaseDownloadState.States ConvertBackTo(string? value, CultureInfo? culture)
    {
        return Enum.TryParse<BaseDownloadState.States>(value, true, out var state)
            ? state
            : BaseDownloadState.States.Unspecified;
    }

    public override string ConvertFrom(BaseDownloadState.States value, CultureInfo? culture)
    {
        return value.ToString();
    }
}
