namespace genzcoders.DTOs.AuthDto;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAtUtc { get; set; }

    public long AccountId { get; set; }
    public string Email { get; set; } = null!;
    public long RoleId { get; set; }
    public string RoleName { get; set; } = null!;
}
