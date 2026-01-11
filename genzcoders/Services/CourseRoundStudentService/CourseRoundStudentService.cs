using genzcoders.DTOs.CourseRoundStudentDto;
using genzcoders.Models;
using genzcoders.Repos.CourseRoundStudentRepo;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Services.CourseRoundStudentService;

public class CourseRoundStudentService : ICourseRoundStudentService
{
    private readonly ICourseRoundStudentRepo _repo;
    private readonly Data.AppDbContext _context;

    public CourseRoundStudentService(ICourseRoundStudentRepo repo, Data.AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<IEnumerable<ReadCourseRoundStudentDto>> GetByCourseRoundIdAsync(long courseRoundId)
    {
        var assignments = await _repo.GetByCourseRoundIdAsync(courseRoundId);
        return assignments.Select(a => new ReadCourseRoundStudentDto
        {
            Id = a.Id,
            CourseRoundId = a.CourseRoundId,
            StudentId = a.StudentId,
            StudentName = a.Student.FullNameEn,
            StudentEmail = a.Student.Email,
            AssignedAt = a.AssignedAt,
            CompletedAt = a.CompletedAt,
            IsActive = a.IsActive
        });
    }

    public async Task<IEnumerable<ReadCourseRoundStudentDto>> GetByStudentIdAsync(long studentId)
    {
        var assignments = await _repo.GetByStudentIdAsync(studentId);
        return assignments.Select(a => new ReadCourseRoundStudentDto
        {
            Id = a.Id,
            CourseRoundId = a.CourseRoundId,
            StudentId = a.StudentId,
            StudentName = a.Student.FullNameEn,
            StudentEmail = a.Student.Email,
            AssignedAt = a.AssignedAt,
            CompletedAt = a.CompletedAt,
            IsActive = a.IsActive
        });
    }

    public async Task<ReadCourseRoundStudentDto> AssignStudentAsync(long courseRoundId, AssignStudentDto dto, long instructorId)
    {
        var courseRound = await _context.CourseRounds
            .FirstOrDefaultAsync(cr => cr.Id == courseRoundId);
        
        if (courseRound == null)
            throw new InvalidOperationException("Course round not found.");
        
        if (courseRound.MainInstructorId != instructorId)
            throw new UnauthorizedAccessException("Only the main instructor can assign students to this course round.");

        // Check if student is already assigned
        var existing = await _repo.GetByCourseRoundAndStudentAsync(courseRoundId, dto.StudentId);
        if (existing != null)
            throw new InvalidOperationException("Student is already assigned to this course round.");

        // Verify student exists
        var student = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == dto.StudentId);
        if (student == null)
            throw new InvalidOperationException("Student not found.");

        var assignment = new CourseRoundStudent
        {
            CourseRoundId = courseRoundId,
            StudentId = dto.StudentId,
            AssignedAt = DateTime.UtcNow,
            IsActive = true
        };

        var created = await _repo.AddAsync(assignment);
        
        return new ReadCourseRoundStudentDto
        {
            Id = created.Id,
            CourseRoundId = created.CourseRoundId,
            StudentId = created.StudentId,
            StudentName = created.Student.FullNameEn,
            StudentEmail = created.Student.Email,
            AssignedAt = created.AssignedAt,
            CompletedAt = created.CompletedAt,
            IsActive = created.IsActive
        };
    }

    public async Task<bool> UnassignStudentAsync(long id, long instructorId)
    {
        var assignment = await _repo.GetByIdAsync(id);
        if (assignment == null) return false;

        var courseRound = await _context.CourseRounds
            .FirstOrDefaultAsync(cr => cr.Id == assignment.CourseRoundId);
        
        if (courseRound == null)
            throw new InvalidOperationException("Course round not found.");
        
        if (courseRound.MainInstructorId != instructorId)
            throw new UnauthorizedAccessException("Only the main instructor can unassign students from this course round.");

        await _repo.DeleteAsync(assignment);
        return true;
    }
}
