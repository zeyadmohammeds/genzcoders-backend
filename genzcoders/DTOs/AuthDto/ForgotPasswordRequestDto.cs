using System.ComponentModel.DataAnnotations;

namespace genzcoders.DTOs.AuthDto;

public class ForgotPasswordRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}
