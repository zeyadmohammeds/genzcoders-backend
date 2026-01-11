namespace genzcoders.DTOs.CourseRoundDto;

public class ReadRoundDto
{
    public long Id { get; set; }
    public int RoundNumber { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public long? MaxStudents { get; set; }
    public long CourseId { get; set; }
    public long InstructorId { get; set; }
    public string Instructor { get; set; } = null!;
    public string Status { get; set; } = null!;
}
