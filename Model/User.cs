using System.Text.Json.Serialization;
using MParchin.Authority.JsonConverter;

namespace MParchin.Authority.Model;

public partial class User
{
    public int Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Role { get; set; }
    [JsonConverter(typeof(DateTimeToEpochConverter))]
    public DateTime? LastLogIn { get; set; }
    [JsonConverter(typeof(DateTimeToEpochConverter))]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    [JsonConverter(typeof(DateTimeToEpochConverter))]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}