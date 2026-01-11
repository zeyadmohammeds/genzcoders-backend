using genzcoders.Models;

namespace genzcoders.Repos.CourseRoundStudentRepo;

public interface ICourseRoundStudentRepo
{
    Task<List<CourseRoundStudent>> GetByCourseRoundIdAsync(long courseRoundId);
    Task<List<CourseRoundStudent>> GetByStudentIdAsync(long studentId);
    Task<CourseRoundStudent?> GetByIdAsync(long id);
    Task<CourseRoundStudent?> GetByCourseRoundAndStudentAsync(long courseRoundId, long studentId);
    Task<CourseRoundStudent> AddAsync(CourseRoundStudent courseRoundStudent);
    Task UpdateAsync(CourseRoundStudent courseRoundStudent);
    Task DeleteAsync(CourseRoundStudent courseRoundStudent);
    Task SaveChangesAsync();
}
