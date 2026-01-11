namespace genzcoders.DTOs.AuthDto;

public class UserProfileDto
{
    public long Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string NationalId { get; set; } = null!;
    public string FullNameEn { get; set; } = null!;
    public string FullNameAr { get; set; } = null!;
    public long RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public DateOnly? CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool HasPassword { get; set; }
    public bool HasGoogleAuth { get; set; }
}
