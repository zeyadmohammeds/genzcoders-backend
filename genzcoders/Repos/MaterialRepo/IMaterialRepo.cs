using genzcoders.Models;

namespace genzcoders.Repos.MaterialRepo;

public interface IMaterialRepo
{
    Task<List<Material>> GetByCourseRoundIdAsync(long courseRoundId);
    Task<Material?> GetByIdAsync(long id);
    Task<Material> AddAsync(Material material);
    Task UpdateAsync(Material material);
    Task DeleteAsync(Material material);
    Task SaveChangesAsync();
}
