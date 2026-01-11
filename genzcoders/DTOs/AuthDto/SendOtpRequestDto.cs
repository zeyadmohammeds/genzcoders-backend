using System.ComponentModel.DataAnnotations;

namespace genzcoders.DTOs.AuthDto;

public class SendOtpRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}
