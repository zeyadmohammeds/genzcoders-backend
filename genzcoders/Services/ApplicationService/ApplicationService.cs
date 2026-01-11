using genzcoders.DTOs.ApplicationDto;
using genzcoders.Data;
using genzcoders.Models;
using genzcoders.Repos.ApplicationRepo;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Services.ApplicationService;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepo _repo;
    private readonly AppDbContext _context;

    public ApplicationService(IApplicationRepo repo, AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<IEnumerable<ReadApplicationDto>> GetAllAsync()
    {
        var apps = await _repo.GetAllAsync();

        return apps.Select(a => new ReadApplicationDto
        {
            Id = a.Id,
            FullName = a.FullName,
            Age = a.Age,
            CourseId = a.CourseId,
            Answer1 = a.Answer1,
            Answer2 = a.Answer2,
            Answer3 = a.Answer3,
            Answer4 = a.Answer4,
            Answer5 = a.Answer5,
            StatusId = a.StatusId,
            AppliedAt = a.ApplicationDate
        });
    }

    public async Task<ReadApplicationDto> CreateAsync(CreateApplicationDto dto)
    {
        // Ensure a status exists - get first status or create a default one
        var status = await _context.Statuses.FirstOrDefaultAsync();
        
        if (status == null)
        {
            // Create a default "Pending" status if none exists
            status = new Status
            {
                StatusName = "Pending"
            };
            _context.Statuses.Add(status);
            await _context.SaveChangesAsync();
        }

        var app = new Application
        {
            FullName = dto.FullName,
            Age = dto.Age,
            CourseId = dto.CourseId,
            ApplicationDate = DateTime.UtcNow,
            StatusId = status.Id,
            Answer1 = dto.Answer1,
            Answer2 = dto.Answer2,
            Answer3 = dto.Answer3,
            Answer4 = dto.Answer4,
            Answer5 = dto.Answer5
        };

        await _repo.AddAsync(app);
        await _repo.SaveChangesAsync();

        return new ReadApplicationDto
        {
            Id = app.Id,
            FullName = app.FullName,
            Age = app.Age,
            CourseId = app.CourseId,
            Answer1 = app.Answer1,
            Answer2 = app.Answer2,
            Answer3 = app.Answer3,
            Answer4 = app.Answer4,
            Answer5 = app.Answer5,
            StatusId = app.StatusId,
            AppliedAt = app.ApplicationDate
        };
    }

    public async Task<ReadApplicationDto?> UpdateStatusAsync(long id, PatchApplicationStatusDto dto)
    {
        var app = await _repo.GetByIdAsync(id);
        if (app == null) return null;

        // Validate that the StatusId exists
        var statusExists = await _context.Statuses.AnyAsync(s => s.Id == dto.StatusId);
        if (!statusExists)
        {
            throw new InvalidOperationException($"Status with ID {dto.StatusId} does not exist.");
        }

        app.StatusId = dto.StatusId;
        await _repo.UpdateAsync(app);

        return new ReadApplicationDto
        {
            Id = app.Id,
            FullName = app.FullName,
            Age = app.Age,
            CourseId = app.CourseId,
            Answer1 = app.Answer1,
            Answer2 = app.Answer2,
            Answer3 = app.Answer3,
            Answer4 = app.Answer4,
            Answer5 = app.Answer5,
            StatusId = app.StatusId,
            AppliedAt = app.ApplicationDate
        };
    }

    public async Task<ReadDetailedApplicationDto?> GetByIdAsync(long id)
    {
        var app = await _repo.GetByIdAsync(id);
        if (app == null) return null;

        return new ReadDetailedApplicationDto
        {
            Id = app.Id,
            FullName = app.FullName,
            Age = app.Age,
            CourseId = app.CourseId,
            Answer1 = app.Answer1,
            Answer2 = app.Answer2,
            Answer3 = app.Answer3,
            Answer4 = app.Answer4,
            Answer5 = app.Answer5,
            StatusId = app.StatusId,
            AppliedAt = app.ApplicationDate
        };
    }

    public async Task UpdateApplicationAsync(long id, UpdateApplicationDto dto)
    {
        var application = await _repo.GetByIdAsync(id);
        if (application == null)
            throw new InvalidOperationException("Application not found.");

        application.FullName = dto.FullName;
        application.Age = dto.Age;
        application.Answer1 = dto.Answer1;
        application.Answer2 = dto.Answer2;
        application.Answer3 = dto.Answer3;
        application.Answer4 = dto.Answer4;
        application.Answer5 = dto.Answer5;

        await _repo.UpdateAsync(application);
    }
}
