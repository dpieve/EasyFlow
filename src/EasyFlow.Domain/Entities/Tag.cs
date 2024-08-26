namespace EasyFlow.Domain.Entities;

public sealed class Tag
{
    public Tag()
    {
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Session> Sessions { get; set; } = [];

    public static readonly int MaxNumTags = 10;
    public bool IsNew()
    {
        return Id == 0;
    }
}
