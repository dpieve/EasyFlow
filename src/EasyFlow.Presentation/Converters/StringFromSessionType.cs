using Avalonia.Data.Converters;
using System.Globalization;
using System;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Features.Dashboard;
using System.Collections.Generic;
using System.Linq;

namespace EasyFlow.Presentation.Converters;
public sealed class StringFromSessionTypes : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable<SessionType> sessionTypes)
        {
            return sessionTypes.Select(sessionType => sessionType.ToCustomString()).ToList();
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable<string> sessionTypes)
        {
            return sessionTypes.Select(sessionType => sessionType.SessionTypeFromString()).ToList();
        }
        return null;
    }
}

public sealed class StringFromSessionType : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SessionType sessionType)
        {
            return sessionType.ToCustomString();
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string sessionType)
        {
            return sessionType.SessionTypeFromString();
        }
        return null;
    }
}
