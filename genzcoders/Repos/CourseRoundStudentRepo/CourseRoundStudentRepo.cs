using genzcoders.Data;
using genzcoders.Models;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Repos.CourseRoundStudentRepo;

public class CourseRoundStudentRepo : ICourseRoundStudentRepo
{
    private readonly AppDbContext _context;

    public CourseRoundStudentRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseRoundStudent>> GetByCourseRoundIdAsync(long courseRoundId)
    {
        return await _context.CourseRoundStudents
            .Include(s => s.Student)
            .Include(s => s.CourseRound)
            .ThenInclude(cr => cr.Course)
            .Where(s => s.CourseRoundId == courseRoundId && s.IsActive)
            .ToListAsync();
    }

    public async Task<List<CourseRoundStudent>> GetByStudentIdAsync(long studentId)
    {
        return await _context.CourseRoundStudents
            .Include(s => s.Student)
            .Include(s => s.CourseRound)
            .ThenInclude(cr => cr.Course)
            .Where(s => s.StudentId == studentId && s.IsActive)
            .ToListAsync();
    }

    public async Task<CourseRoundStudent?> GetByIdAsync(long id)
    {
        return await _context.CourseRoundStudents
            .Include(s => s.Student)
            .Include(s => s.CourseRound)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<CourseRoundStudent?> GetByCourseRoundAndStudentAsync(long courseRoundId, long studentId)
    {
        return await _context.CourseRoundStudents
            .Include(s => s.Student)
            .Include(s => s.CourseRound)
            .FirstOrDefaultAsync(s => s.CourseRoundId == courseRoundId && s.StudentId == studentId);
    }

    public async Task<CourseRoundStudent> AddAsync(CourseRoundStudent courseRoundStudent)
    {
        await _context.CourseRoundStudents.AddAsync(courseRoundStudent);
        await _context.SaveChangesAsync();
        return courseRoundStudent;
    }

    public async Task UpdateAsync(CourseRoundStudent courseRoundStudent)
    {
        _context.CourseRoundStudents.Update(courseRoundStudent);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(CourseRoundStudent courseRoundStudent)
    {
        _context.CourseRoundStudents.Remove(courseRoundStudent);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
