namespace genzcoders.DTOs.AuthDto;

public class RegisterRequestDto
{
    public string NationalId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Phone { get; set; }
    public string FullNameEn { get; set; } = null!;
    public string FullNameAr { get; set; } = null!;
}
