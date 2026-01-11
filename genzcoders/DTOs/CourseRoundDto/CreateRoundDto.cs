namespace genzcoders.DTOs.CourseRoundDto;

public class CreateRoundDto
{
    public int RoundNumber { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public long? MaxStudents { get; set; }
    public long MainInstructorId { get; set; }
    public long StatusId { get; set; } = 0; // Optional, will use default if 0
    public long CourseId { get; set; }

    public string? Question1 { get; set; }
    public string? Question2 { get; set; }
    public string? Question3 { get; set; }
    public string? Question4 { get; set; }
    public string? Question5 { get; set; }
}
