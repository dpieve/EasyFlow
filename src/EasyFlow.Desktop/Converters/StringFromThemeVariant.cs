using Avalonia.Data.Converters;
using Avalonia.Styling;
using EasyFlow.Desktop.Services;
using System;
using System.Globalization;

namespace EasyFlow.Desktop.Converters;
public class StringFromThemeVariant : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ThemeVariant tv)
        {
            if (tv.ToString() == "Dark")
            {
                return ConstantTranslation.ThemeLight;
            }

            return ConstantTranslation.ThemeDark;
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string theme)
        {
            if (theme == ConstantTranslation.ThemeDark)
            {
                return ThemeVariant.Light;
            }

            return ThemeVariant.Dark;
        }

        return null;
    }
}
