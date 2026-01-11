using System.Text.Json.Serialization;

namespace genzcoders.DTOs.CoursesDto;

public class ReadOneDetailedCourse
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;
    
    [JsonPropertyName("levelId")]
    public long LevelId { get; set; }
    
    [JsonPropertyName("durationHours")]
    public long DurationHours { get; set; }
    
    [JsonPropertyName("maxStudents")]
    public long? MaxStudents { get; set; }
    
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }
    
    // Optional fields
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("category")]
    public string? Category { get; set; }
    
    [JsonPropertyName("instructor")]
    public string? Instructor { get; set; }
    
    [JsonPropertyName("instructorId")]
    public string? InstructorId { get; set; }
    
    [JsonPropertyName("students")]
    public int? Students { get; set; }
    
    [JsonPropertyName("rating")]
    public decimal? Rating { get; set; }
    
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
}
