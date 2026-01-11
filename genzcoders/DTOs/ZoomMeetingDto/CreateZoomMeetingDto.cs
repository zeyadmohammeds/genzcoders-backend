namespace genzcoders.DTOs.ZoomMeetingDto;

public class CreateZoomMeetingDto
{
    public long CourseRoundId { get; set; }
    public string Topic { get; set; } = null!;
    public string? Description { get; set; }
    public string MeetingLink { get; set; } = null!;
    public string? MeetingId { get; set; }
    public string? Passcode { get; set; }
    public DateTime MeetingDateTime { get; set; }
    public int DurationMinutes { get; set; }
}
