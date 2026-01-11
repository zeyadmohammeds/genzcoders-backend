using System.ComponentModel.DataAnnotations;

namespace genzcoders.DTOs.AuthDto;

public class ResetPasswordRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Token { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; set; } = null!;
}
