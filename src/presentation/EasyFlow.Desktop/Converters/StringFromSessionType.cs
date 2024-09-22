using Avalonia.Data.Converters;
using EasyFlow.Desktop.Features.Dashboard;
using EasyFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EasyFlow.Desktop.Converters;

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