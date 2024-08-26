namespace EasyFlow.Domain.Entities;

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
    public string Description { get; set; } = string.Empty;
}

public enum SessionType
{
    Focus,
    Break,
    LongBreak
}