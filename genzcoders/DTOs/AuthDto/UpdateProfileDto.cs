using System.ComponentModel.DataAnnotations;

namespace genzcoders.DTOs.AuthDto;

public class UpdateProfileDto
{
    [EmailAddress]
    public string? Email { get; set; }

    public string? FullNameEn { get; set; }

    public string? FullNameAr { get; set; }

    public string? Phone { get; set; }

    public string? NationalId { get; set; }
}
