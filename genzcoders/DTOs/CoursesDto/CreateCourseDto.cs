namespace genzcoders.DTOs.CoursesDto;

public class CreateCourseDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long LevelId { get; set; }
    public long DurationHours { get; set; }
    public long? MaxStudents { get; set; }
    public decimal? Price { get; set; }
    public List<long> InstructorIds { get; set; } = new List<long>();
}
