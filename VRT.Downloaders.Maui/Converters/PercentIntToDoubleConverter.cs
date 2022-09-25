using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace VRT.Downloaders.Maui.Converters;

/// <summary>
/// Converts an integer value to double precission percent number
/// </summary>
public class PercentIntToDoubleConverter : BaseConverter<int, double>
{
    public override double DefaultConvertReturnValue { get; set; }
    public override int DefaultConvertBackReturnValue { get; set; }

    public override int ConvertBackTo(double value, CultureInfo culture)
    {
        return (int)value * 100;
    }

    public override double ConvertFrom(int value, CultureInfo culture)
    {
        return value / (double)100;
    }
}