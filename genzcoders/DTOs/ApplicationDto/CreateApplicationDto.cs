namespace genzcoders.DTOs.ApplicationDto;

public class CreateApplicationDto
{
    public string FullName { get; set; } = null!;
    public int Age { get; set; }
    public int CourseId { get; set; }

    public string? Answer1 { get; set; }
    public string? Answer2 { get; set; }
    public string? Answer3 { get; set; }
    public string? Answer4 { get; set; }
    public string? Answer5 { get; set; }
}
