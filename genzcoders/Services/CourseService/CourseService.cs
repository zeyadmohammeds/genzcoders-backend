using genzcoders.DTOs.CoursesDto;
using genzcoders.Models;
using genzcoders.Repos.CourseRepo;
using genzcoders.Data;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Services.CourseService;

public class CourseService : ICourseService
{
    private readonly ICourseRepo _repo;
    private readonly AppDbContext _context;

    public CourseService(ICourseRepo repo, AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<ReadCourseDto> CreateCourseAsync(CreateCourseDto dto)
    {
        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            LevelId = dto.LevelId,
            DurationHours = dto.DurationHours,
            MaxStudents = dto.MaxStudents,
            Price = dto.Price
        };

        await _repo.AddAsync(course);
        await _repo.SaveChangesAsync();

        // Add instructors if provided
        if (dto.InstructorIds != null && dto.InstructorIds.Any())
        {
            // Validate that all instructor IDs exist and have role ID 2
            var validInstructors = await _context.Accounts
                .Where(a => dto.InstructorIds.Contains(a.Id) && a.RoleId == 2 && a.IsActive)
                .ToListAsync();

            var courseInstructors = validInstructors.Select(instructor => new CourseInstructor
            {
                CourseId = course.Id,
                InstructorId = instructor.Id,
                AssignedAt = DateTime.UtcNow
            }).ToList();

            if (courseInstructors.Any())
            {
                _context.CourseInstructors.AddRange(courseInstructors);
                await _context.SaveChangesAsync();
            }
        }

        return new ReadCourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            LevelId = course.LevelId,
            DurationHours = course.DurationHours,
            MaxStudents = course.MaxStudents,
            Price = course.Price
        };
    }

    public async Task<IEnumerable<ReadCourseDto>> GetAllCourses()
    {
        var courses = await _repo.GetAll();
        return courses.Select(c => new ReadCourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            LevelId = c.LevelId,
            DurationHours = c.DurationHours,
            MaxStudents = c.MaxStudents,
            Price = c.Price
        }).ToList();
    }

    public async Task<ReadOneDetailedCourse?> GetCourseByIdAsync(long id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return null;

        return new ReadOneDetailedCourse
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            LevelId = c.LevelId,
            DurationHours = c.DurationHours,
            MaxStudents = c.MaxStudents,
            Price = c.Price
        };
    }

    public async Task<bool> PatchCourseAsync(long id, PatchCourseDto dto)
    {
        var course = await _repo.GetByIdAsync(id);
        if (course == null) return false;

        if (dto.Title != null) course.Title = dto.Title;
        if (dto.Description != null) course.Description = dto.Description;
        if (dto.LevelId.HasValue) course.LevelId = dto.LevelId.Value;
        if (dto.DurationHours.HasValue) course.DurationHours = dto.DurationHours.Value;
        if (dto.MaxStudents.HasValue) course.MaxStudents = dto.MaxStudents;
        if (dto.Price.HasValue) course.Price = dto.Price;

        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<ReadCourseDto?> UpdateCourseAsync(long id, UpdateCourseDto dto)
    {
        var course = await _repo.GetByIdAsync(id);
        if (course == null) return null;

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.LevelId = dto.LevelId;
        course.DurationHours = dto.DurationHours;
        course.MaxStudents = dto.MaxStudents;
        course.Price = dto.Price;

        await _repo.SaveChangesAsync();

        return new ReadCourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            LevelId = course.LevelId,
            DurationHours = course.DurationHours,
            MaxStudents = course.MaxStudents,
            Price = course.Price
        };
    }

    public async Task<bool> DeleteCourseAsync(long id)
    {
        var course = await _repo.GetByIdAsync(id);
        if (course == null) return false;

        await _repo.DeleteAsync(course);
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ReadCourseDto>> SearchCoursesByNameAsync(string name)
    {
        var courses = await _repo.SearchByNameAsync(name);
        return courses.Select(c => new ReadCourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            LevelId = c.LevelId,
            DurationHours = c.DurationHours,
            MaxStudents = c.MaxStudents,
            Price = c.Price
        });
    }

    public async Task<IEnumerable<ReadCourseDto>> GetCoursesByInstructorAsync(long instructorId)
    {
        var courseIds = await _context.CourseInstructors
            .Where(ci => ci.InstructorId == instructorId)
            .Select(ci => ci.CourseId)
            .ToListAsync();

        var courses = await _repo.GetByIdsAsync(courseIds);
        return courses.Select(c => new ReadCourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            LevelId = c.LevelId,
            DurationHours = c.DurationHours,
            MaxStudents = c.MaxStudents,
            Price = c.Price
        });
    }
}
