using genzcoders.Data;
using genzcoders.Models;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Repos.RoundRepo;

public class RoundRepo : IRoundRepo
{
    private readonly AppDbContext _context;

    public RoundRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CourseRound?> GetByIdAsync(long id)
    {
        return await _context.CourseRounds
            .Include(i => i.MainInstructor)
            .Include(s => s.Status)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<CourseRound>> GetAllAsync()
    {
        return await _context.CourseRounds
            .Include(i => i.MainInstructor)
            .Include(s => s.Status)
            .ToListAsync();
    }

    public async Task AddAsync(CourseRound courseRound)
    {
        await _context.CourseRounds.AddAsync(courseRound);
    }

    public async Task DeleteAsync(CourseRound round)
    {
        _context.CourseRounds.Remove(round);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
