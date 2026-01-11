namespace genzcoders.Models;

public class Level
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
