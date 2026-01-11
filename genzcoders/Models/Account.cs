namespace genzcoders.Models;

public class Account
{
    public long Id { get; set; }
    public string NationalId { get; set; } = null!;
    public string? PasswordHash { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public long RoleId { get; set; }
    public string FullNameEn { get; set; } = null!;
    public string FullNameAr { get; set; } = null!;
    public DateOnly? CreatedAt { get; set; }    
    public bool IsActive { get; set; }
    public long StatusId { get; set; }
    
    // Google OAuth fields
    public string? GoogleId { get; set; }
    public string? GoogleEmail { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    
    // Security fields
    public DateTime? LastLoginAt { get; set; }
    public DateTime? PasswordChangedAt { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiresAt { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
    
    // Two-factor authentication
    public bool TwoFactorEnabled { get; set; }
    public string? TwoFactorSecret { get; set; }

    public Role Role { get; set; } = null!;
    public Status Status { get; set; } = null!;

    public ICollection<CourseRound> CourseRounds { get; set; } = new List<CourseRound>();
    public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
    public ICollection<CourseRoundStudent> CourseRoundStudents { get; set; } = new List<CourseRoundStudent>();
}
