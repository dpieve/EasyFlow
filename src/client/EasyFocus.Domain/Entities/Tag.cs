namespace EasyFocus.Domain.Entities;

public sealed class Tag
{
    public Tag()
    { }

    public int Id { get; set; }
    public string Name { get; init; } = string.Empty;

    public static readonly int MaxNumTags = 10;
    public static readonly int MinNumTags = 1;

    public static Tag CreateTag(string name, int? id = null)
    {
        return new Tag
        {
            Id = id ?? 0,
            Name = name
        };
    }
}