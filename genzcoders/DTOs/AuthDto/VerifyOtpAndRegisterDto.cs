using System.ComponentModel.DataAnnotations;

namespace genzcoders.DTOs.AuthDto;

public class VerifyOtpAndRegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string OtpCode { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = null!;

    [Required]
    public string NationalId { get; set; } = null!;

    [Required]
    public string FullNameEn { get; set; } = null!;

    [Required]
    public string FullNameAr { get; set; } = null!;

    public string? Phone { get; set; }
}
