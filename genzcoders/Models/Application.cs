namespace genzcoders.Models;

public class Application
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public DateTime ApplicationDate { get; set; }
    public long StatusId { get; set; }

    public string? Answer1 { get; set; }
    public string? Answer2 { get; set; }
    public string? Answer3 { get; set; }
    public string? Answer4 { get; set; }
    public string? Answer5 { get; set; }
    public string? Answer6 { get; set; }
    public string? Answer7 { get; set; }
    public string? Answer8 { get; set; }
    public string? Answer9 { get; set; }
    public string? Answer10 { get; set; }

    public string FullName { get; set; } = null!;
    public int Age { get; set; }

    public Course Course { get; set; } = null!;
    public Status Status { get; set; } = null!;
}
