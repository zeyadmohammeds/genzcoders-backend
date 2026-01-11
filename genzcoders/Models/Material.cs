namespace genzcoders.Models;

public class Material
{
    public long Id { get; set; }
    public long CourseRoundId { get; set; }
    public long InstructorId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Link { get; set; } = null!; // URL to the material
    public string MaterialType { get; set; } = "link"; // link, pdf, video, document, etc.
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public CourseRound CourseRound { get; set; } = null!;
    public Account Instructor { get; set; } = null!;
}
