namespace genzcoders.DTOs.WeekDto;

public class UpdateWeekDto
{
    public string Title { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    // Optional: if not provided, existing value will be preserved.
    public string? BusinessEntityName { get; set; }
}
