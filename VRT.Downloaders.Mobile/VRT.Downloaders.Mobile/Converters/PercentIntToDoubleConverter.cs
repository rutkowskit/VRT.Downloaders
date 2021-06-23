using System;
using Xamarin.Forms;

namespace VRT.Downloaders.Mobile.Converters
{
    /// <summary>
    /// Converts an integer value to double precission percent number
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