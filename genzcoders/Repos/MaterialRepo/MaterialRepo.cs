using genzcoders.Data;
using genzcoders.Models;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Repos.MaterialRepo;

public class MaterialRepo : IMaterialRepo
{
    private readonly AppDbContext _context;

    public MaterialRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Material>> GetByCourseRoundIdAsync(long courseRoundId)
    {
        return await _context.Materials
            .Include(m => m.Instructor)
            .Include(m => m.CourseRound)
            .Where(m => m.CourseRoundId == courseRoundId && m.IsActive)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<Material?> GetByIdAsync(long id)
    {
        return await _context.Materials
            .Include(m => m.Instructor)
            .Include(m => m.CourseRound)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Material> AddAsync(Material material)
    {
        await _context.Materials.AddAsync(material);
        await _context.SaveChangesAsync();
        return material;
    }

    public async Task UpdateAsync(Material material)
    {
        material.UpdatedAt = DateTime.UtcNow;
        _context.Materials.Update(material);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Material material)
    {
        _context.Materials.Remove(material);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
