using genzcoders.DTOs.CourseRoundStudentDto;

namespace genzcoders.Services.CourseRoundStudentService;

public interface ICourseRoundStudentService
{
    Task<IEnumerable<ReadCourseRoundStudentDto>> GetByCourseRoundIdAsync(long courseRoundId);
    Task<IEnumerable<ReadCourseRoundStudentDto>> GetByStudentIdAsync(long studentId);
    Task<ReadCourseRoundStudentDto> AssignStudentAsync(long courseRoundId, AssignStudentDto dto, long instructorId);
    Task<bool> UnassignStudentAsync(long id, long instructorId);
}
