using genzcoders.Data;
using genzcoders.Models;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Repos.ApplicationRepo;

public class ApplicationRepo : IApplicationRepo
{
    private readonly AppDbContext _context;

    public ApplicationRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Application>> GetAllAsync()
    {
        return await _context.Applications
            .Include(s => s.Status)
            .Include(c => c.Course)
            .ToListAsync();
    }

    public async Task<Application?> GetByIdAsync(long id)
    {
        return await _context.Applications
            .Include(s => s.Status)
            .Include(c => c.Course)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Application application)
    {
        await _context.Applications.AddAsync(application);
    }

    public async Task UpdateAsync(Application application)
    {
        _context.Applications.Update(application);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
