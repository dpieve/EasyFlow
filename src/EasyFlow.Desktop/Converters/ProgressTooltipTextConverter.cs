using Avalonia.Data.Converters;
using EasyFlow.Desktop.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace EasyFlow.Desktop.Converters;
public sealed class ProgressTooltipTextConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        try { 
            if (values is null)
            {
                return null;
            }

            if (values.Count != 2)
            {
                return null;
            }

            if (values[0] is not int completedTimers)
            {
                return null;
            }

            if (values[1] is not int beforeLongBreak)
            {
                return null;
            }

            var str = ConstantTranslation.ProgressLongBreakToolTip;
            var formatted = string.Format(str, completedTimers, beforeLongBreak);
            return formatted;
        }
        catch(Exception ex)
        {
            Log.Error(ex, "Failed to convert progress tooltip text");
            return null;
        }
    }
}
