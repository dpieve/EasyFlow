using EasyFlow.Data;

namespace EasyFlow.Features.Focus;

public sealed record TimerSettings(int MinimumMinutes, int MaximumMinutes, int TotalMinutes, int DeltaMinutes);

public sealed class FocusSettings
{
    public FocusSettings(
        TimerSettings workTime,
        TimerSettings breakTime,
        TimerSettings longBreakTime,
        Tag tag,
        int timerBeforeLongBreak)
    {
        WorkTime = workTime;
        BreakTime = breakTime;
        LongBreakTime = longBreakTime;
        Tag = tag;
        TimersBeforeLongBreak = timerBeforeLongBreak;
    }

    public TimerSettings WorkTime { get; }
    public TimerSettings BreakTime { get; }
    public TimerSettings LongBreakTime { get; }
    public Tag Tag { get; }
    public int TimersBeforeLongBreak { get; set; }
}