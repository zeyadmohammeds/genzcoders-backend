namespace genzcoders.Models;

public class CourseRoundStudent
{
    public long Id { get; set; }
    public long CourseRoundId { get; set; }
    public long StudentId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public CourseRound CourseRound { get; set; } = null!;
    public Account Student { get; set; } = null!;
}
