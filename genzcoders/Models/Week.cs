namespace genzcoders.Models;

public class Week
{
    public long Id { get; set; }
    public long CourseRoundId { get; set; }

    public string Title { get; set; } = null!;

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public string BusinessEntityName { get; set; } = "genzcoders";

    public CourseRound CourseRound { get; set; } = null!;
}
