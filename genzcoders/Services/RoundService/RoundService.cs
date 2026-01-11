using genzcoders.DTOs.CourseRoundDto;
using genzcoders.Models;
using genzcoders.Repos.RoundRepo;
using genzcoders.Data;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Services.RoundService;

public class RoundService : IRoundService
{
    private readonly IRoundRepo _repo;
    private readonly AppDbContext _context;

    public RoundService(IRoundRepo repo, AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<IEnumerable<ReadRoundDto>> GetAllAsync()
    {
        var rounds = await _repo.GetAllAsync();
        return rounds.Select(r => new ReadRoundDto
        {
            Id = r.Id,
            RoundNumber = r.RoundNumber,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            MaxStudents = r.MaxStudents,
            CourseId = r.CourseId,
            Instructor = r.MainInstructor.FullNameEn,
            InstructorId = r.MainInstructorId,
            Status = r.Status.StatusName
        });
    }

    public async Task<ReadOneDetailedRound?> GetByIdAsync(long id)
    {
        var r = await _repo.GetByIdAsync(id);
        if (r == null) return null;

        return new ReadOneDetailedRound
        {
            Id = r.Id,
            RoundNumber = r.RoundNumber,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            MaxStudents = r.MaxStudents,
            CourseId = r.CourseId,
            Instructor = r.MainInstructor.FullNameEn,
            Status = r.Status.StatusName,
            Question1 = r.Question1,
            Question2 = r.Question2,
            Question3 = r.Question3,
            Question4 = r.Question4,
            Question5 = r.Question5
        };
    }

    public async Task<long> CreateAsync(CreateRoundDto dto)
    {
        // Validate StatusId exists, or get first available status
        long statusId = dto.StatusId;
        var statusExists = await _context.Statuses.AnyAsync(s => s.Id == statusId);
        if (!statusExists)
        {
            var firstStatus = await _context.Statuses.FirstOrDefaultAsync();
            if (firstStatus == null)
                throw new InvalidOperationException("No statuses found in database. Please add at least one status.");
            statusId = firstStatus.Id;
        }

        var round = new CourseRound
        {
            RoundNumber = dto.RoundNumber,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            MaxStudents = dto.MaxStudents,
            MainInstructorId = dto.MainInstructorId,
            StatusId = statusId,
            CourseId = dto.CourseId,
            CreatedAt = DateTime.UtcNow,
            Question1 = dto.Question1,
            Question2 = dto.Question2,
            Question3 = dto.Question3,
            Question4 = dto.Question4,
            Question5 = dto.Question5
        };

        await _repo.AddAsync(round);
        await _repo.SaveChangesAsync();
        return round.Id;
    }

    public async Task<bool> UpdateAsync(long id, UpdateRoundDto dto)
    {
        var round = await _repo.GetByIdAsync(id);
        if (round == null) return false;

        round.RoundNumber = dto.RoundNumber;
        round.StartDate = dto.StartDate;
        round.EndDate = dto.EndDate;
        round.MaxStudents = dto.MaxStudents;
        round.MainInstructorId = dto.MainInstructorId;
        round.StatusId = dto.StatusId;
        round.Question1 = dto.Question1;
        round.Question2 = dto.Question2;
        round.Question3 = dto.Question3;
        round.Question4 = dto.Question4;
        round.Question5 = dto.Question5;

        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PatchAsync(long id, PatchRoundDto dto)
    {
        var round = await _repo.GetByIdAsync(id);
        if (round == null) return false;

        if (dto.RoundNumber.HasValue) round.RoundNumber = dto.RoundNumber.Value;
        if (dto.StartDate.HasValue) round.StartDate = dto.StartDate.Value;
        if (dto.EndDate.HasValue) round.EndDate = dto.EndDate.Value;
        if (dto.MaxStudents.HasValue) round.MaxStudents = dto.MaxStudents;
        if (dto.MainInstructorId.HasValue) round.MainInstructorId = dto.MainInstructorId.Value;
        if (dto.StatusId.HasValue) round.StatusId = dto.StatusId.Value;

        if (dto.Question1 != null) round.Question1 = dto.Question1;
        if (dto.Question2 != null) round.Question2 = dto.Question2;
        if (dto.Question3 != null) round.Question3 = dto.Question3;
        if (dto.Question4 != null) round.Question4 = dto.Question4;
        if (dto.Question5 != null) round.Question5 = dto.Question5;

        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var round = await _repo.GetByIdAsync(id);
        if (round == null) return false;

        await _repo.DeleteAsync(round);
        await _repo.SaveChangesAsync();
        return true;
    }
}
