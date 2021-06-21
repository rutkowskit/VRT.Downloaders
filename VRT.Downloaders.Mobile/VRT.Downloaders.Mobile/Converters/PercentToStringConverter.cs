using System;
using Xamarin.Forms;

namespace VRT.Downloaders.Mobile.Converters
{
    /// <summary>
    /// Converts an incoming value using all of the incoming converters in sequence.
    /// </summary>
    public class PercentIntToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => value is int intValue
            ? (intValue / (double)100)
            : value;

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => value is double dValue
            ? (int)dValue * 100
            : value;
    }
}