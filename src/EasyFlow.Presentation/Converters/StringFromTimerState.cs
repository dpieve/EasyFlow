using Avalonia.Data.Converters;
using EasyFlow.Presentation.Features.Focus.RunningTimer;
using System;
using System.Globalization;

namespace EasyFlow.Presentation.Converters;

public sealed class StringFromTimerState : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimerState timerState)
        {
            return timerState switch
            {
                TimerState.Focus => "Focus",
                TimerState.Break => "Break",
                TimerState.LongBreak => "Long Break",
                _ => null,
            };
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string timerStateString)
        {
            return timerStateString switch
            {
                "Focus" => TimerState.Focus,
                "Break" => TimerState.Break,
                "Long Break" => TimerState.LongBreak,
                _ => null,
            };
        }
        return null;
    }
}