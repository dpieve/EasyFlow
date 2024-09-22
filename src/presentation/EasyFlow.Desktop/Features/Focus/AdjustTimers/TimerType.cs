using System.Collections.Immutable;

namespace EasyFlow.Desktop.Features.Focus.AdjustTimers;

public enum TimerType
{
    Work,
    Break,
    LongBreak
}

public enum AdjustFactor
{
    StepForward,
    LongStepForward,
    StepBackward,
    LongStepBackward
}

public sealed record TimerTypeLimit(int Min, int Max, int Step, int LongStep);

public static partial class Constants
{
    public static readonly IImmutableDictionary<TimerType, TimerTypeLimit> TimerTypeLimits =
        ImmutableDictionary.Create<TimerType, TimerTypeLimit>()
            .Add(TimerType.Work, new TimerTypeLimit(Min: 5, Max: 999, Step: 1, LongStep: 10))
            .Add(TimerType.Break, new TimerTypeLimit(Min: 1, Max: 999, Step: 1, LongStep: 5))
            .Add(TimerType.LongBreak, new TimerTypeLimit(Min: 2, Max: 999, Step: 1, LongStep: 5));
}