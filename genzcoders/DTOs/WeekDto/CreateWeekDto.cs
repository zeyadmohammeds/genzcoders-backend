namespace genzcoders.DTOs.WeekDto;

public class CreateWeekDto
{
    public long CourseRoundId { get; set; }
    public string Title { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    // Optional for "other system" usage. In our system it will default to "genzcoders".
    public string? BusinessEntityName { get; set; }
}
