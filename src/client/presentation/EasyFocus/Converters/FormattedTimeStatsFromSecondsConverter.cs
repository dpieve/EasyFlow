using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace EasyFocus.Converters;

public sealed class FormattedTimeStatsFromSecondsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int secondsLeft)
        {
            return string.Empty;
        }
        var days = secondsLeft / 86400;
        var hours = secondsLeft / 3600;
        var minutes = secondsLeft / 60;
        var seconds = secondsLeft % 60;

        if (days > 0)
        {
            return $"{days}d {hours}h {minutes}m {seconds}s";
        }

        if (hours > 0)
        {
            return $"{hours}h {minutes}m {seconds}s";
        }

        return $"{minutes}m {seconds}s";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}