namespace EasyFocus.Domain.Entities;

public sealed record DisplaySettings(
    SessionType SessionType,
    FilterPeriod FilterPeriod,
    string FilterText);