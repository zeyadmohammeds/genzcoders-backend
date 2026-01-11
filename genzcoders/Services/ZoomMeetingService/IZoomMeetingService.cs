using genzcoders.DTOs.ZoomMeetingDto;

namespace genzcoders.Services.ZoomMeetingService;

public interface IZoomMeetingService
{
    Task<IEnumerable<ReadZoomMeetingDto>> GetByCourseRoundIdAsync(long courseRoundId);
    Task<ReadZoomMeetingDto?> GetByIdAsync(long id);
    Task<ReadZoomMeetingDto> CreateAsync(CreateZoomMeetingDto dto, long instructorId);
    Task<ReadZoomMeetingDto?> UpdateAsync(long id, UpdateZoomMeetingDto dto, long instructorId);
    Task<bool> DeleteAsync(long id, long instructorId);
}
