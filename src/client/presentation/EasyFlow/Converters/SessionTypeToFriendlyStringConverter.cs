using Avalonia.Data.Converters;
using EasyFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EasyFlow.Converters;

public sealed class SessionTypesToFriendlyStringsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable<SessionType> sessionTypes)
        {
            return sessionTypes.Select(sessionType => sessionType.ToFriendlyString()).ToList();
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IEnumerable<string> sessionTypes)
        {
            return sessionTypes.Select(sessionType => sessionType.ToSessionType()).ToList();
        }
        return null;
    }
}

public sealed class SessionTypeToFriendlyStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SessionType sessionType)
        {
            return sessionType.ToFriendlyString();
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string sessionType)
        {
            return sessionType.ToSessionType();
        }
        return null;
    }
}