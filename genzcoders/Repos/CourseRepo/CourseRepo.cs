using genzcoders.Data;
using genzcoders.Models;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Repos.CourseRepo;

public class CourseRepo : ICourseRepo
{
    private readonly AppDbContext _context;

    public CourseRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
    }

    public async Task<IEnumerable<Course>> GetAll()
    {
        return await _context.Courses.Include(l => l.Level).ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(long id)
    {
        return await _context.Courses.Include(l => l.Level).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Course>> GetByIdsAsync(List<long> ids)
    {
        return await _context.Courses
            .Include(l => l.Level)
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();
    }

    public async Task<List<Course>> SearchByNameAsync(string name)
    {
        return await _context.Courses
            .Include(l => l.Level)
            .Where(c => c.Title.Contains(name))
            .ToListAsync();
    }

    public async Task DeleteAsync(Course course)
    {
        _context.Courses.Remove(course);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
