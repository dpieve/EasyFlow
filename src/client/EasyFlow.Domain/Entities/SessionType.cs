namespace EasyFlow.Domain.Entities;

public enum SessionType
{
    None,
    Pomodoro,
    ShortBreak,
    LongBreak
}

public static class SessionTypeExtensions
{
    public static string ToFriendlyString(this SessionType sessionType)
    {
        return sessionType switch
        {
            SessionType.None => "All types",
            SessionType.Pomodoro => "Pomodoro",
            SessionType.ShortBreak => "Short Break",
            SessionType.LongBreak => "Long Break",
            _ => throw new ArgumentOutOfRangeException(nameof(sessionType), sessionType, null)
        };
    }

    public static SessionType ToSessionType(this string sessionTypeString)
    {
        return sessionTypeString switch
        {
            "All types" => SessionType.None,
            "Pomodoro" => SessionType.Pomodoro,
            "Short Break" => SessionType.ShortBreak,
            "Long Break" => SessionType.LongBreak,
            _ => throw new ArgumentOutOfRangeException(nameof(sessionTypeString), sessionTypeString, null)
        };
    }

    public static bool IsBreak(this SessionType sessionType)
    {
        return sessionType == SessionType.ShortBreak || sessionType == SessionType.LongBreak;
    }
}