using Avalonia.Data.Converters;
using EasyFlow.Presentation.Features.Dashboard.DisplayControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EasyFlow.Presentation.Converters;

public sealed class StringFromDisplayTypes : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable<DisplayType> displayTypes)
        {
            return displayTypes.Select(type => type.ToCustomString()).ToList();
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable<string> displayTypes)
        {
            return displayTypes.Select(type => type.DisplayTypeFromString()).ToList();
        }
        return null;
    }
}

public sealed class StringFromDisplayType : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DisplayType type)
        {
            return type.ToCustomString();
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string type)
        {
            return type.DisplayTypeFromString();
        }

        return null;
    }
}