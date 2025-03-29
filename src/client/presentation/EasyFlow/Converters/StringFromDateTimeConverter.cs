using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace EasyFlow.Converters;

public sealed class StringFromDateTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            var formatted = date.ToString("dd-MM-yyyy");
            return formatted;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            DateTime.TryParseExact(stringValue, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
            return result;
        }

        return null;
    }
}