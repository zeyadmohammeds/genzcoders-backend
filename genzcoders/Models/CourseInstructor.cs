namespace genzcoders.Models;

public class CourseInstructor
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public long InstructorId { get; set; }
    public DateTime AssignedAt { get; set; }

    public Course Course { get; set; } = null!;
    public Account Instructor { get; set; } = null!;
}
