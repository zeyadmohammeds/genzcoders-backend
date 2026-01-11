using genzcoders.Models;

namespace genzcoders.Repos.WeekRepo;

public interface IWeekRepo
{
    Task<List<Week>> GetByBusinessEntityNameAsync(string businessEntityName);
    Task<List<Week>> GetByCourseRoundIdAsync(long courseRoundId, string businessEntityName);
    Task<Week?> GetByIdAsync(long id);
    Task<Week> AddAsync(Week week);
    Task UpdateAsync(Week week);
    Task DeleteAsync(Week week);
}
