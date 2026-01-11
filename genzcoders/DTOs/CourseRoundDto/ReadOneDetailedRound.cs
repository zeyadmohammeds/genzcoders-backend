namespace genzcoders.DTOs.CourseRoundDto;

public class ReadOneDetailedRound
{
    public long Id { get; set; }
    public int RoundNumber { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public long? MaxStudents { get; set; }
    public long CourseId { get; set; }
    public string Instructor { get; set; } = null!;
    public string Status { get; set; } = null!;

    public string? Question1 { get; set; }
    public string? Question2 { get; set; }
    public string? Question3 { get; set; }
    public string? Question4 { get; set; }
    public string? Question5 { get; set; }
}
