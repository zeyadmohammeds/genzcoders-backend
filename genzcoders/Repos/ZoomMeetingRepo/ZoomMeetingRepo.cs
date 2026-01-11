using genzcoders.Data;
using genzcoders.Models;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Repos.ZoomMeetingRepo;

public class ZoomMeetingRepo : IZoomMeetingRepo
{
    private readonly AppDbContext _context;

    public ZoomMeetingRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ZoomMeeting>> GetByCourseRoundIdAsync(long courseRoundId)
    {
        return await _context.ZoomMeetings
            .Include(z => z.Instructor)
            .Include(z => z.CourseRound)
            .Where(z => z.CourseRoundId == courseRoundId && z.IsActive)
            .OrderBy(z => z.MeetingDateTime)
            .ToListAsync();
    }

    public async Task<ZoomMeeting?> GetByIdAsync(long id)
    {
        return await _context.ZoomMeetings
            .Include(z => z.Instructor)
            .Include(z => z.CourseRound)
            .FirstOrDefaultAsync(z => z.Id == id);
    }

    public async Task<ZoomMeeting> AddAsync(ZoomMeeting zoomMeeting)
    {
        await _context.ZoomMeetings.AddAsync(zoomMeeting);
        await _context.SaveChangesAsync();
        return zoomMeeting;
    }

    public async Task UpdateAsync(ZoomMeeting zoomMeeting)
    {
        zoomMeeting.UpdatedAt = DateTime.UtcNow;
        _context.ZoomMeetings.Update(zoomMeeting);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ZoomMeeting zoomMeeting)
    {
        _context.ZoomMeetings.Remove(zoomMeeting);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
