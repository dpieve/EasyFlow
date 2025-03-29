using EasyFlow.Api.Sessions;
using System.Text.Json.Serialization;

namespace EasyFlow.Api.Tags;

public sealed class Tag
{
    public int Id { get; set;  }
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public ICollection<Session> Sessions { get; set; } = [];
}