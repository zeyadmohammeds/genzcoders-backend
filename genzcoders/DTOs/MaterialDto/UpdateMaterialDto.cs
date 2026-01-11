namespace genzcoders.DTOs.MaterialDto;

public class UpdateMaterialDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Link { get; set; } = null!;
    public string MaterialType { get; set; } = "link";
    public bool IsActive { get; set; } = true;
}
