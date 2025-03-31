namespace EasyFocus.Domain.Entities;

public sealed class Session
{
    public Session()
    { }

    public int Id { get; set; }
    public DateTime FinishedDateTime { get; init; }
    public int DurationSeconds { get; init; }
    public int CompletedSeconds { get; init; }
    public SessionType SessionType { get; init; } = SessionType.None;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted => CompletedSeconds >= DurationSeconds;

    public string TagName { get; init; } = string.Empty;

    public static Session CreateSession(
        int durationSeconds,
        int completedSeconds,
        SessionType sessionType,
        Tag tag,
        DateTime? finishedDateTime = null)
    {
        var date = finishedDateTime ?? DateTime.Now;

        return new Session
        {
            FinishedDateTime = date,
            DurationSeconds = durationSeconds,
            CompletedSeconds = completedSeconds,
            SessionType = sessionType,
            TagName = tag.Name
        };
    }
}

public static class SessionExtensions
{
    public static bool ApplyFilter(this Session session, string filterText)
    {
        filterText = filterText.Trim();

        if (string.IsNullOrEmpty(filterText) || string.IsNullOrWhiteSpace(filterText))
        {
            return true;
        }

        return session.TagName.Contains(filterText, StringComparison.OrdinalIgnoreCase) ||
                session.Description.Contains(filterText, StringComparison.OrdinalIgnoreCase) ||
                session.SessionType.ToFriendlyString().Contains(filterText, StringComparison.OrdinalIgnoreCase);
    }
}