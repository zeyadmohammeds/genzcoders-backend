using genzcoders.DTOs.ApplicationDto;

namespace genzcoders.Services.ApplicationService;

public interface IApplicationService
{
    Task<IEnumerable<ReadApplicationDto>> GetAllAsync();
    Task<ReadApplicationDto> CreateAsync(CreateApplicationDto dto);
    Task<ReadApplicationDto?> UpdateStatusAsync(long id, PatchApplicationStatusDto dto);
    Task UpdateApplicationAsync(long id, UpdateApplicationDto dto);
    Task<ReadDetailedApplicationDto?> GetByIdAsync(long id);
}
