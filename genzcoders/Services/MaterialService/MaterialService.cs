using genzcoders.DTOs.MaterialDto;
using genzcoders.Models;
using genzcoders.Repos.MaterialRepo;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Services.MaterialService;

public class MaterialService : IMaterialService
{
    private readonly IMaterialRepo _repo;
    private readonly Data.AppDbContext _context;

    public MaterialService(IMaterialRepo repo, Data.AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<IEnumerable<ReadMaterialDto>> GetByCourseRoundIdAsync(long courseRoundId)
    {
        var materials = await _repo.GetByCourseRoundIdAsync(courseRoundId);
        return materials.Select(m => new ReadMaterialDto
        {
            Id = m.Id,
            CourseRoundId = m.CourseRoundId,
            InstructorId = m.InstructorId,
            InstructorName = m.Instructor.FullNameEn,
            Title = m.Title,
            Description = m.Description,
            Link = m.Link,
            MaterialType = m.MaterialType,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt,
            IsActive = m.IsActive
        });
    }

    public async Task<ReadMaterialDto?> GetByIdAsync(long id)
    {
        var material = await _repo.GetByIdAsync(id);
        if (material == null) return null;

        return new ReadMaterialDto
        {
            Id = material.Id,
            CourseRoundId = material.CourseRoundId,
            InstructorId = material.InstructorId,
            InstructorName = material.Instructor.FullNameEn,
            Title = material.Title,
            Description = material.Description,
            Link = material.Link,
            MaterialType = material.MaterialType,
            CreatedAt = material.CreatedAt,
            UpdatedAt = material.UpdatedAt,
            IsActive = material.IsActive
        };
    }

    public async Task<ReadMaterialDto> CreateAsync(CreateMaterialDto dto, long instructorId)
    {
        // Verify course round exists and instructor is the main instructor
        var courseRound = await _context.CourseRounds
            .FirstOrDefaultAsync(cr => cr.Id == dto.CourseRoundId);
        
        if (courseRound == null)
            throw new InvalidOperationException("Course round not found.");
        
        if (courseRound.MainInstructorId != instructorId)
            throw new UnauthorizedAccessException("Only the main instructor can add materials to this course round.");

        var material = new Material
        {
            CourseRoundId = dto.CourseRoundId,
            InstructorId = instructorId,
            Title = dto.Title,
            Description = dto.Description,
            Link = dto.Link,
            MaterialType = dto.MaterialType,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var created = await _repo.AddAsync(material);
        
        return new ReadMaterialDto
        {
            Id = created.Id,
            CourseRoundId = created.CourseRoundId,
            InstructorId = created.InstructorId,
            InstructorName = created.Instructor.FullNameEn,
            Title = created.Title,
            Description = created.Description,
            Link = created.Link,
            MaterialType = created.MaterialType,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt,
            IsActive = created.IsActive
        };
    }

    public async Task<ReadMaterialDto?> UpdateAsync(long id, UpdateMaterialDto dto, long instructorId)
    {
        var material = await _repo.GetByIdAsync(id);
        if (material == null) return null;

        if (material.InstructorId != instructorId)
            throw new UnauthorizedAccessException("Only the instructor who created the material can update it.");

        material.Title = dto.Title;
        material.Description = dto.Description;
        material.Link = dto.Link;
        material.MaterialType = dto.MaterialType;
        material.IsActive = dto.IsActive;

        await _repo.UpdateAsync(material);

        return new ReadMaterialDto
        {
            Id = material.Id,
            CourseRoundId = material.CourseRoundId,
            InstructorId = material.InstructorId,
            InstructorName = material.Instructor.FullNameEn,
            Title = material.Title,
            Description = material.Description,
            Link = material.Link,
            MaterialType = material.MaterialType,
            CreatedAt = material.CreatedAt,
            UpdatedAt = material.UpdatedAt,
            IsActive = material.IsActive
        };
    }

    public async Task<bool> DeleteAsync(long id, long instructorId)
    {
        var material = await _repo.GetByIdAsync(id);
        if (material == null) return false;

        if (material.InstructorId != instructorId)
            throw new UnauthorizedAccessException("Only the instructor who created the material can delete it.");

        await _repo.DeleteAsync(material);
        return true;
    }
}
