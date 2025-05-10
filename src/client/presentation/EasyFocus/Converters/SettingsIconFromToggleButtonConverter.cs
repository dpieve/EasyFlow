using Avalonia.Data.Converters;
using Avalonia.Media;
using EasyFocus.Resources;
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

        return !isChecked
            ? AppIcons.SettingsOpen
            : AppIcons.SettingsClose;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}