namespace genzcoders.Models;

public class EmailOtp
{
    public long Id { get; set; }
    public string Email { get; set; } = null!;
    public string OtpCode { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public int Attempts { get; set; }
}
