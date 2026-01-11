using genzcoders.Models;

namespace genzcoders.Repos.RoundRepo;

public interface IRoundRepo
{
    Task<CourseRound?> GetByIdAsync(long id);
    Task<List<CourseRound>> GetAllAsync();
    Task AddAsync(CourseRound courseRound);
    Task DeleteAsync(CourseRound round);
    Task SaveChangesAsync();
}
