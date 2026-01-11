using genzcoders.Data;
using genzcoders.Models;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Repos.WeekRepo;

public class WeekRepo : IWeekRepo
{
    private readonly AppDbContext _context;

    public WeekRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Week>> GetByBusinessEntityNameAsync(string businessEntityName)
    {
        return await _context.Weeks
            .Include(w => w.CourseRound)
            .Where(w => w.BusinessEntityName == businessEntityName)
            .OrderBy(w => w.StartDate)
            .ToListAsync();
    }

    public async Task<List<Week>> GetByCourseRoundIdAsync(long courseRoundId, string businessEntityName)
    {
        return await _context.Weeks
            .Include(w => w.CourseRound)
            .Where(w => w.CourseRoundId == courseRoundId && w.BusinessEntityName == businessEntityName)
            .OrderBy(w => w.StartDate)
            .ToListAsync();
    }

    public async Task<Week?> GetByIdAsync(long id)
    {
        return await _context.Weeks
            .Include(w => w.CourseRound)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<Week> AddAsync(Week week)
    {
        await _context.Weeks.AddAsync(week);
        await _context.SaveChangesAsync();
        return week;
    }

    public async Task UpdateAsync(Week week)
    {
        _context.Weeks.Update(week);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Week week)
    {
        _context.Weeks.Remove(week);
        await _context.SaveChangesAsync();
    }
}
