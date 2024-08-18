using System.Collections.Immutable;

namespace EasyFlow.Features.Focus.AdjustTimers;
public enum TimerType
{
    Work,
    Break,
    LongBreak
}

public enum AdjustFactor
{
    Decrease = -1,
    Increase = 1,
}

public sealed record TimerTypeLimit(int Min, int Max, int Delta);

public static partial class Constants
{
    public static readonly IImmutableDictionary<TimerType, TimerTypeLimit> TimerTypeLimits = 
        ImmutableDictionary.Create<TimerType, TimerTypeLimit>()
            .Add(TimerType.Work, new TimerTypeLimit(Min: 1, Max: 40, Delta: 1))
            .Add(TimerType.Break, new TimerTypeLimit(Min: 1, Max: 40, Delta: 1))
            .Add(TimerType.LongBreak, new TimerTypeLimit(Min: 1, Max: 40, Delta: 1));
}
