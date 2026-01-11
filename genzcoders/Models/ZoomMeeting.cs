namespace genzcoders.Models;

public class ZoomMeeting
{
    public long Id { get; set; }
    public long CourseRoundId { get; set; }
    public long InstructorId { get; set; }
    public string Topic { get; set; } = null!;
    public string? Description { get; set; }
    public string MeetingLink { get; set; } = null!; // Zoom meeting URL
    public string? MeetingId { get; set; } // Zoom meeting ID
    public string? Passcode { get; set; } // Meeting password/passcode
    public DateTime MeetingDateTime { get; set; } // Scheduled meeting date/time
    public int DurationMinutes { get; set; } // Duration in minutes
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public CourseRound CourseRound { get; set; } = null!;
    public Account Instructor { get; set; } = null!;
}
