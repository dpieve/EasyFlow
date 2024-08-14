using System;

namespace EasyFlow.Data;

public sealed class Session
{
    public Session()
    {
    }

    public int Id { get; set; }
    public int DurationMinutes { get; set; }
    public SessionType SessionType { get; set; }
    public DateTime FinishedDate { get; set; }

    public int TagId { get; set; }
    public Tag Tag { get; set; }
}

public enum SessionType
{
    Work,
    Break,
    LongBreak
}