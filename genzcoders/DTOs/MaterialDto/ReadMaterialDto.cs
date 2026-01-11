namespace genzcoders.DTOs.MaterialDto;

public class ReadMaterialDto
{
    public long Id { get; set; }
    public long CourseRoundId { get; set; }
    public long InstructorId { get; set; }
    public string InstructorName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Link { get; set; } = null!;
    public string MaterialType { get; set; } = "link";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
