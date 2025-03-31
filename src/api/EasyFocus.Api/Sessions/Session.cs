using EasyFocus.Api.Tags;

namespace EasyFocus.Api.Sessions;

public sealed class Session
{
    public int Id { get; set; }
    public DateTime FinishedDateTime { get; set; }
    public int DurationSeconds { get; set; }
    public int CompletedSeconds { get; set; }
    public SessionType SessionType { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted => CompletedSeconds >= DurationSeconds;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}