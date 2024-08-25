using System.Collections.Generic;

namespace EasyFlow.Presentation.Data;

public sealed class Tag
{
    public Tag()
    {
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Session> Sessions { get; set; } = [];

    public bool IsNew()
    {
        return Id == 0;
    }

    public static readonly int MaxNumTags = 10;
}