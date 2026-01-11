using genzcoders.DTOs.WeekDto;

namespace genzcoders.Services.WeekService;

public interface IWeekService
{
    Task<IEnumerable<ReadWeekDto>> GetByBusinessEntityNameAsync(string businessEntityName);
    Task<IEnumerable<ReadWeekDto>> GetByCourseRoundIdAsync(long courseRoundId, string businessEntityName);
    Task<ReadWeekDto?> GetByIdAsync(long id);
    Task<ReadWeekDto> CreateAsync(CreateWeekDto dto);
    Task<ReadWeekDto?> UpdateAsync(long id, UpdateWeekDto dto);
    Task<bool> DeleteAsync(long id);
}
