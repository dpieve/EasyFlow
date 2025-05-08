using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace EasyFocus.Converters;

public sealed class StringFromDateTimeConverter : IValueConverter
{
    private const string _format = "dd/MM/yyyy H:m";
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            var formatted = date.ToString(_format);
            return formatted;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            DateTime.TryParseExact(stringValue, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
            return result;
        }

        return null;
    }
}