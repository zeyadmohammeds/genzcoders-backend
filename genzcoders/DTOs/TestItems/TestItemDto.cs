namespace genzcoders.DTOs.TestItems;

public class TestItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
