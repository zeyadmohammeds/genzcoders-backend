using genzcoders.Models;

namespace genzcoders.Repos.ZoomMeetingRepo;

public interface IZoomMeetingRepo
{
    Task<List<ZoomMeeting>> GetByCourseRoundIdAsync(long courseRoundId);
    Task<ZoomMeeting?> GetByIdAsync(long id);
    Task<ZoomMeeting> AddAsync(ZoomMeeting zoomMeeting);
    Task UpdateAsync(ZoomMeeting zoomMeeting);
    Task DeleteAsync(ZoomMeeting zoomMeeting);
    Task SaveChangesAsync();
}
