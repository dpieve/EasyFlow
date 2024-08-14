using System.Collections.Generic;

namespace EasyFlow.Data;

public sealed class Tag
{
    public Tag()
    {
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Session> Sessions { get; set; } = [];
}