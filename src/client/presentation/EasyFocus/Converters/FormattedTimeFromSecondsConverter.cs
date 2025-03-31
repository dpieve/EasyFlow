using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace EasyFocus.Converters;

public sealed class FormattedTimeFromSecondsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int secondsLeft)
        {
            return string.Empty;
        }
        var minutes = secondsLeft / 60;
        var seconds = secondsLeft % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}