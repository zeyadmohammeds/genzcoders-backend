using genzcoders.Models;

namespace genzcoders.Repos.CourseRepo;

public interface ICourseRepo
{
    Task<IEnumerable<Course>> GetAll();
    Task<Course?> GetByIdAsync(long id);
    Task<IEnumerable<Course>> GetByIdsAsync(List<long> ids);
    Task AddAsync(Course course);
    Task SaveChangesAsync();
    Task<List<Course>> SearchByNameAsync(string name);
    Task DeleteAsync(Course course);
}
