namespace genzcoders.DTOs.CoursesDto;

public class PatchCourseDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public long? LevelId { get; set; }
    public long? DurationHours { get; set; }
    public long? MaxStudents { get; set; }
    public decimal? Price { get; set; }
}
