namespace genzcoders.DTOs.CourseRoundStudentDto;

public class ReadCourseRoundStudentDto
{
    public long Id { get; set; }
    public long CourseRoundId { get; set; }
    public long StudentId { get; set; }
    public string StudentName { get; set; } = null!;
    public string StudentEmail { get; set; } = null!;
    public DateTime AssignedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsActive { get; set; }
}
