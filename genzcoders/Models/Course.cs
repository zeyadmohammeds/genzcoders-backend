namespace genzcoders.Models;

public class Course
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long LevelId { get; set; }
    public long DurationHours { get; set; }
    public long? MaxStudents { get; set; }
    public decimal? Price { get; set; }

    public Level Level { get; set; } = null!;
    public ICollection<CourseRound> CourseRounds { get; set; } = new List<CourseRound>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
}
