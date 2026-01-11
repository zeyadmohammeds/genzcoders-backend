using genzcoders.DTOs.ZoomMeetingDto;
using genzcoders.Models;
using genzcoders.Repos.ZoomMeetingRepo;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Services.ZoomMeetingService;

public class ZoomMeetingService : IZoomMeetingService
{
    private readonly IZoomMeetingRepo _repo;
    private readonly Data.AppDbContext _context;

    public ZoomMeetingService(IZoomMeetingRepo repo, Data.AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<IEnumerable<ReadZoomMeetingDto>> GetByCourseRoundIdAsync(long courseRoundId)
    {
        var meetings = await _repo.GetByCourseRoundIdAsync(courseRoundId);
        return meetings.Select(m => new ReadZoomMeetingDto
        {
            Id = m.Id,
            CourseRoundId = m.CourseRoundId,
            InstructorId = m.InstructorId,
            InstructorName = m.Instructor.FullNameEn,
            Topic = m.Topic,
            Description = m.Description,
            MeetingLink = m.MeetingLink,
            MeetingId = m.MeetingId,
            Passcode = m.Passcode,
            MeetingDateTime = m.MeetingDateTime,
            DurationMinutes = m.DurationMinutes,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt,
            IsActive = m.IsActive
        });
    }

    public async Task<ReadZoomMeetingDto?> GetByIdAsync(long id)
    {
        var meeting = await _repo.GetByIdAsync(id);
        if (meeting == null) return null;

        return new ReadZoomMeetingDto
        {
            Id = meeting.Id,
            CourseRoundId = meeting.CourseRoundId,
            InstructorId = meeting.InstructorId,
            InstructorName = meeting.Instructor.FullNameEn,
            Topic = meeting.Topic,
            Description = meeting.Description,
            MeetingLink = meeting.MeetingLink,
            MeetingId = meeting.MeetingId,
            Passcode = meeting.Passcode,
            MeetingDateTime = meeting.MeetingDateTime,
            DurationMinutes = meeting.DurationMinutes,
            CreatedAt = meeting.CreatedAt,
            UpdatedAt = meeting.UpdatedAt,
            IsActive = meeting.IsActive
        };
    }

    public async Task<ReadZoomMeetingDto> CreateAsync(CreateZoomMeetingDto dto, long instructorId)
    {
        var courseRound = await _context.CourseRounds
            .FirstOrDefaultAsync(cr => cr.Id == dto.CourseRoundId);
        
        if (courseRound == null)
            throw new InvalidOperationException("Course round not found.");
        
        if (courseRound.MainInstructorId != instructorId)
            throw new UnauthorizedAccessException("Only the main instructor can add zoom meetings to this course round.");

        var meeting = new ZoomMeeting
        {
            CourseRoundId = dto.CourseRoundId,
            InstructorId = instructorId,
            Topic = dto.Topic,
            Description = dto.Description,
            MeetingLink = dto.MeetingLink,
            MeetingId = dto.MeetingId,
            Passcode = dto.Passcode,
            MeetingDateTime = dto.MeetingDateTime,
            DurationMinutes = dto.DurationMinutes,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var created = await _repo.AddAsync(meeting);
        
        return new ReadZoomMeetingDto
        {
            Id = created.Id,
            CourseRoundId = created.CourseRoundId,
            InstructorId = created.InstructorId,
            InstructorName = created.Instructor.FullNameEn,
            Topic = created.Topic,
            Description = created.Description,
            MeetingLink = created.MeetingLink,
            MeetingId = created.MeetingId,
            Passcode = created.Passcode,
            MeetingDateTime = created.MeetingDateTime,
            DurationMinutes = created.DurationMinutes,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt,
            IsActive = created.IsActive
        };
    }

    public async Task<ReadZoomMeetingDto?> UpdateAsync(long id, UpdateZoomMeetingDto dto, long instructorId)
    {
        var meeting = await _repo.GetByIdAsync(id);
        if (meeting == null) return null;

        if (meeting.InstructorId != instructorId)
            throw new UnauthorizedAccessException("Only the instructor who created the zoom meeting can update it.");

        meeting.Topic = dto.Topic;
        meeting.Description = dto.Description;
        meeting.MeetingLink = dto.MeetingLink;
        meeting.MeetingId = dto.MeetingId;
        meeting.Passcode = dto.Passcode;
        meeting.MeetingDateTime = dto.MeetingDateTime;
        meeting.DurationMinutes = dto.DurationMinutes;
        meeting.IsActive = dto.IsActive;

        await _repo.UpdateAsync(meeting);

        return new ReadZoomMeetingDto
        {
            Id = meeting.Id,
            CourseRoundId = meeting.CourseRoundId,
            InstructorId = meeting.InstructorId,
            InstructorName = meeting.Instructor.FullNameEn,
            Topic = meeting.Topic,
            Description = meeting.Description,
            MeetingLink = meeting.MeetingLink,
            MeetingId = meeting.MeetingId,
            Passcode = meeting.Passcode,
            MeetingDateTime = meeting.MeetingDateTime,
            DurationMinutes = meeting.DurationMinutes,
            CreatedAt = meeting.CreatedAt,
            UpdatedAt = meeting.UpdatedAt,
            IsActive = meeting.IsActive
        };
    }

    public async Task<bool> DeleteAsync(long id, long instructorId)
    {
        var meeting = await _repo.GetByIdAsync(id);
        if (meeting == null) return false;

        if (meeting.InstructorId != instructorId)
            throw new UnauthorizedAccessException("Only the instructor who created the zoom meeting can delete it.");

        await _repo.DeleteAsync(meeting);
        return true;
    }
}
