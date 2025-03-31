using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace EasyFocus.Converters;

public sealed class SettingsIconFromToggleButtonConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool isChecked)
        {
            return string.Empty;
        }

        StreamGeometry geometry =
            !isChecked
            ? StreamGeometry.Parse("M2 4.5C2 4.22386 2.22386 4 2.5 4H17.5C17.7761 4 18 4.22386 18 4.5C18 4.77614 17.7761 5 17.5 5H2.5C2.22386 5 2 4.77614 2 4.5Z M2 9.5C2 9.22386 2.22386 9 2.5 9H17.5C17.7761 9 18 9.22386 18 9.5C18 9.77614 17.7761 10 17.5 10H2.5C2.22386 10 2 9.77614 2 9.5Z M2.5 14C2.22386 14 2 14.2239 2 14.5C2 14.7761 2.22386 15 2.5 15H17.5C17.7761 15 18 14.7761 18 14.5C18 14.2239 17.7761 14 17.5 14H2.5Z")
            : StreamGeometry.Parse("M 16.6 4.4 C 16.9 4.7 16.9 5.2 16.6 5.5 L 11.6 11.5 L 16.6 17.5 C 16.9 17.8 16.9 18.3 16.6 18.6 C 16.3 18.9 15.8 18.9 15.5 18.6 L 10.5 12.6 L 5.5 18.6 C 5.2 18.9 4.7 18.9 4.4 18.6 C 4.1 18.3 4.1 17.8 4.4 17.5 L 9.4 11.5 L 4.4 5.5 C 4.1 5.2 4.1 4.7 4.4 4.4 C 4.7 4.1 5.2 4.1 5.5 4.4 L 10.5 10.4 L 15.5 4.4 C 15.8 4.1 16.3 4.1 16.6 4.4 Z");
        return geometry;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}