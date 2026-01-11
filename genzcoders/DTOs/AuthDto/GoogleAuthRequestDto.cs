using System.ComponentModel.DataAnnotations;

namespace genzcoders.DTOs.AuthDto;

public class GoogleAuthRequestDto
{
    [Required]
    public string IdToken { get; set; } = null!;
}
