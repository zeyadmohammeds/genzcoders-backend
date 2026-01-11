using System.Text.Json.Serialization;

namespace genzcoders.DTOs.ApplicationDto;

public class PatchApplicationStatusDto
{
    [JsonPropertyName("statusId")]
    public long StatusId { get; set; }
}
