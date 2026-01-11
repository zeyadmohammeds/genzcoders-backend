using genzcoders.DTOs.MaterialDto;

namespace genzcoders.Services.MaterialService;

public interface IMaterialService
{
    Task<IEnumerable<ReadMaterialDto>> GetByCourseRoundIdAsync(long courseRoundId);
    Task<ReadMaterialDto?> GetByIdAsync(long id);
    Task<ReadMaterialDto> CreateAsync(CreateMaterialDto dto, long instructorId);
    Task<ReadMaterialDto?> UpdateAsync(long id, UpdateMaterialDto dto, long instructorId);
    Task<bool> DeleteAsync(long id, long instructorId);
}
