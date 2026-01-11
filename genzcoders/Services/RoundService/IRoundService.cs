using genzcoders.DTOs.CourseRoundDto;

namespace genzcoders.Services.RoundService;

public interface IRoundService
{
    Task<IEnumerable<ReadRoundDto>> GetAllAsync();
    Task<ReadOneDetailedRound?> GetByIdAsync(long id);
    Task<long> CreateAsync(CreateRoundDto dto);
    Task<bool> UpdateAsync(long id, UpdateRoundDto dto);
    Task<bool> PatchAsync(long id, PatchRoundDto dto);
    Task<bool> DeleteAsync(long id);
}
