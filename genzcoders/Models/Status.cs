namespace genzcoders.Models;

public class Status
{
    public long Id { get; set; }
    public string StatusName { get; set; } = null!;

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<CourseRound> CourseRounds { get; set; } = new List<CourseRound>();
}
