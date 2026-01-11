using genzcoders.DTOs.CoursesDto;

namespace genzcoders.Services.CourseService;

public interface ICourseService
{
    Task<IEnumerable<ReadCourseDto>> GetAllCourses();
    Task<ReadOneDetailedCourse?> GetCourseByIdAsync(long id);
    Task<ReadCourseDto> CreateCourseAsync(CreateCourseDto dto);
    Task<ReadCourseDto?> UpdateCourseAsync(long id, UpdateCourseDto dto);
    Task<bool> PatchCourseAsync(long id, PatchCourseDto dto);
    Task<bool> DeleteCourseAsync(long id);
    Task<IEnumerable<ReadCourseDto>> SearchCoursesByNameAsync(string name);
    Task<IEnumerable<ReadCourseDto>> GetCoursesByInstructorAsync(long instructorId);
}
