using System.Text.Json.Serialization;

namespace genzcoders.DTOs.ApplicationDto;

public class ReadDetailedApplicationDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = null!;
    
    [JsonPropertyName("age")]
    public int Age { get; set; }
    
    [JsonPropertyName("courseId")]
    public long CourseId { get; set; }
    
    [JsonPropertyName("answer1")]
    public string? Answer1 { get; set; }
    
    [JsonPropertyName("answer2")]
    public string? Answer2 { get; set; }
    
    [JsonPropertyName("answer3")]
    public string? Answer3 { get; set; }
    
    [JsonPropertyName("answer4")]
    public string? Answer4 { get; set; }
    
    [JsonPropertyName("answer5")]
    public string? Answer5 { get; set; }
    
    [JsonPropertyName("statusId")]
    public long? StatusId { get; set; }
    
    [JsonPropertyName("studentId")]
    public string? StudentId { get; set; }
    
    [JsonPropertyName("appliedAt")]
    public DateTime? AppliedAt { get; set; }
    
    [JsonPropertyName("reviewedAt")]
    public DateTime? ReviewedAt { get; set; }
    
    [JsonPropertyName("reviewedBy")]
    public string? ReviewedBy { get; set; }
    
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
