using genzcoders.DTOs.WeekDto;
using genzcoders.Models;
using genzcoders.Repos.WeekRepo;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Services.WeekService;

public class WeekService : IWeekService
{
    private readonly IWeekRepo _repo;
    private readonly Data.AppDbContext _context;

    public WeekService(IWeekRepo repo, Data.AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<IEnumerable<ReadWeekDto>> GetByBusinessEntityNameAsync(string businessEntityName)
    {
        var name = string.IsNullOrWhiteSpace(businessEntityName) ? "genzcoders" : businessEntityName.Trim();
        var weeks = await _repo.GetByBusinessEntityNameAsync(name);
        return weeks.Select(MapToReadDto);
    }

    public async Task<IEnumerable<ReadWeekDto>> GetByCourseRoundIdAsync(long courseRoundId, string businessEntityName)
    {
        var name = string.IsNullOrWhiteSpace(businessEntityName) ? "genzcoders" : businessEntityName.Trim();
        var weeks = await _repo.GetByCourseRoundIdAsync(courseRoundId, name);
        return weeks.Select(MapToReadDto);
    }

    public async Task<ReadWeekDto?> GetByIdAsync(long id)
    {
        var week = await _repo.GetByIdAsync(id);
        return week == null ? null : MapToReadDto(week);
    }

    public async Task<ReadWeekDto> CreateAsync(CreateWeekDto dto)
    {
        var courseRoundExists = await _context.CourseRounds.AnyAsync(r => r.Id == dto.CourseRoundId);
        if (!courseRoundExists)
            throw new InvalidOperationException("Course round not found.");

        if (dto.EndDate < dto.StartDate)
            throw new InvalidOperationException("EndDate must be after StartDate.");

        var entity = new Week
        {
            CourseRoundId = dto.CourseRoundId,
            Title = dto.Title,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            BusinessEntityName = string.IsNullOrWhiteSpace(dto.BusinessEntityName) ? "genzcoders" : dto.BusinessEntityName.Trim(),
        };

        var created = await _repo.AddAsync(entity);
        return MapToReadDto(created);
    }

    public async Task<ReadWeekDto?> UpdateAsync(long id, UpdateWeekDto dto)
    {
        var week = await _repo.GetByIdAsync(id);
        if (week == null) return null;

        if (dto.EndDate < dto.StartDate)
            throw new InvalidOperationException("EndDate must be after StartDate.");

        week.Title = dto.Title;
        week.StartDate = dto.StartDate;
        week.EndDate = dto.EndDate;
        if (!string.IsNullOrWhiteSpace(dto.BusinessEntityName))
            week.BusinessEntityName = dto.BusinessEntityName.Trim();

        await _repo.UpdateAsync(week);
        return MapToReadDto(week);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var week = await _repo.GetByIdAsync(id);
        if (week == null) return false;
        await _repo.DeleteAsync(week);
        return true;
    }

    private static ReadWeekDto MapToReadDto(Week w) => new()
    {
        Id = w.Id,
        CourseRoundId = w.CourseRoundId,
        Title = w.Title,
        StartDate = w.StartDate,
        EndDate = w.EndDate,
        BusinessEntityName = w.BusinessEntityName,
    };
}
