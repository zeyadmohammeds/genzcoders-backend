using genzcoders.Models;

namespace genzcoders.Repos.ApplicationRepo;

public interface IApplicationRepo
{
    Task<List<Application>> GetAllAsync();
    Task<Application?> GetByIdAsync(long id);
    Task AddAsync(Application application);
    Task UpdateAsync(Application application);
    Task SaveChangesAsync();
}
