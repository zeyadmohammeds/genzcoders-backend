namespace genzcoders.DTOs.MaterialDto;

public class CreateMaterialDto
{
    public long CourseRoundId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Link { get; set; } = null!;
    public string MaterialType { get; set; } = "link";
}
