using Avalonia.Data.Converters;
using EasyFlow.Presentation.Services;
using System;
using System.Globalization;

namespace EasyFlow.Presentation.Converters;

public sealed class StringFromDateTime : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            var format = LanguageService.GetDateFormat();
            var formatted = date.ToString(format);
            return formatted;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            var format = LanguageService.GetDateFormat();
            DateTime.TryParseExact(stringValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
            return result;
        }

        return null;
    }
}