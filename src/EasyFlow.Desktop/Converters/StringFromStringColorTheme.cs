using Avalonia.Data.Converters;
using EasyFlow.Desktop.Services;
using System.Globalization;

namespace EasyFlow.Desktop.Converters;

public sealed class StringFromStringColorTheme : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string colorTheme)
        {
            switch (colorTheme)
            {
                case "Orange":
                    return ConstantTranslation.ThemeOrange;

                case "Red":
                    return ConstantTranslation.ThemeRed;

                case "Green":
                    return ConstantTranslation.ThemeGreen;

                case "Blue":
                    return ConstantTranslation.ThemeBlue;
            }
        }

        return ConstantTranslation.ThemeRed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (string?)value;
    }
}