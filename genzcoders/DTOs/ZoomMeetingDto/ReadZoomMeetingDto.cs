namespace genzcoders.DTOs.ZoomMeetingDto;

public class ReadZoomMeetingDto
{
    public long Id { get; set; }
    public long CourseRoundId { get; set; }
    public long InstructorId { get; set; }
    public string InstructorName { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public string? Description { get; set; }
    public string MeetingLink { get; set; } = null!;
    public string? MeetingId { get; set; }
    public string? Passcode { get; set; }
    public DateTime MeetingDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
