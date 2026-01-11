using genzcoders.Data;
using genzcoders.DTOs.AuthDto;
using genzcoders.Helpers;
using genzcoders.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace genzcoders.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<Account> _passwordHasher;
    private readonly ILogger<AdminController> _logger;

    public AdminController(AppDbContext db, IPasswordHasher<Account> passwordHasher, ILogger<AdminController> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    private async Task<bool> IsAdminAsync(CancellationToken cancellationToken = default)
    {
        var accountIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(accountIdClaim) || !long.TryParse(accountIdClaim, out long accountId))
            return false;

        var account = await _db.Accounts
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        return account != null && account.Role.RoleName.Equals(RoleHelper.Admin, StringComparison.OrdinalIgnoreCase);
    }

    [HttpPost("create-instructor")]
    public async Task<ActionResult> CreateInstructor([FromBody] CreateAdminOrInstructorDto dto, CancellationToken cancellationToken = default)
    {
        if (!await IsAdminAsync(cancellationToken))
            return Forbid("Only administrators can create instructor accounts.");

        var email = dto.Email.Trim().ToLowerInvariant();

        // Validate email
        if (!IsValidEmail(email))
            return BadRequest("Invalid email format.");

        // Check if email already exists
        var existing = await _db.Accounts.AsNoTracking().AnyAsync(a => a.Email == email, cancellationToken);
        if (existing)
            return Conflict("Email already exists.");

        // Validate password strength
        var passwordValidation = ValidatePassword(dto.Password);
        if (!passwordValidation.IsValid)
            return BadRequest(passwordValidation.ErrorMessage);

        // Get instructor role
        var roleId = await RoleHelper.GetRoleIdByNameAsync(_db, RoleHelper.Instructor, cancellationToken);
        if (!roleId.HasValue || roleId.Value == 0)
            return StatusCode(StatusCodes.Status500InternalServerError, "Instructor role not found in database.");

        var statusId = await _db.Statuses.AsNoTracking().OrderBy(s => s.Id).Select(s => s.Id).FirstOrDefaultAsync(cancellationToken);
        if (statusId == 0)
            return StatusCode(StatusCodes.Status500InternalServerError, "No statuses found in database.");

        var account = new Account
        {
            NationalId = dto.NationalId,
            Email = email,
            Phone = dto.Phone,
            FullNameEn = dto.FullNameEn,
            FullNameAr = dto.FullNameAr,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            RoleId = roleId.Value,
            StatusId = statusId,
            IsActive = true,
            IsEmailVerified = false,
            PasswordChangedAt = DateTime.UtcNow
        };

        account.PasswordHash = _passwordHasher.HashPassword(account, dto.Password);

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Instructor account created by admin: {Email}", email);
        return Ok(new { message = "Instructor account created successfully.", id = account.Id, email = account.Email });
    }

    [HttpPost("create-admin")]
    public async Task<ActionResult> CreateAdmin([FromBody] CreateAdminOrInstructorDto dto, CancellationToken cancellationToken = default)
    {
        if (!await IsAdminAsync(cancellationToken))
            return Forbid("Only administrators can create admin accounts.");

        var email = dto.Email.Trim().ToLowerInvariant();

        // Validate email
        if (!IsValidEmail(email))
            return BadRequest("Invalid email format.");

        // Check if email already exists
        var existing = await _db.Accounts.AsNoTracking().AnyAsync(a => a.Email == email, cancellationToken);
        if (existing)
            return Conflict("Email already exists.");

        // Validate password strength
        var passwordValidation = ValidatePassword(dto.Password);
        if (!passwordValidation.IsValid)
            return BadRequest(passwordValidation.ErrorMessage);

        // Get admin role
        var roleId = await RoleHelper.GetRoleIdByNameAsync(_db, RoleHelper.Admin, cancellationToken);
        if (!roleId.HasValue || roleId.Value == 0)
            return StatusCode(StatusCodes.Status500InternalServerError, "Admin role not found in database.");

        var statusId = await _db.Statuses.AsNoTracking().OrderBy(s => s.Id).Select(s => s.Id).FirstOrDefaultAsync(cancellationToken);
        if (statusId == 0)
            return StatusCode(StatusCodes.Status500InternalServerError, "No statuses found in database.");

        var account = new Account
        {
            NationalId = dto.NationalId,
            Email = email,
            Phone = dto.Phone,
            FullNameEn = dto.FullNameEn,
            FullNameAr = dto.FullNameAr,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            RoleId = roleId.Value,
            StatusId = statusId,
            IsActive = true,
            IsEmailVerified = false,
            PasswordChangedAt = DateTime.UtcNow
        };

        account.PasswordHash = _passwordHasher.HashPassword(account, dto.Password);

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Admin account created by admin: {Email}", email);
        return Ok(new { message = "Admin account created successfully.", id = account.Id, email = account.Email });
    }

    [HttpGet("users")]
    public async Task<ActionResult> GetAllUsers(CancellationToken cancellationToken = default)
    {
        if (!await IsAdminAsync(cancellationToken))
            return Forbid("Only administrators can view all users.");

        var users = await _db.Accounts
            .Include(a => a.Role)
            .Include(a => a.Status)
            .Select(a => new
            {
                id = a.Id,
                email = a.Email,
                fullNameEn = a.FullNameEn,
                fullNameAr = a.FullNameAr,
                phone = a.Phone,
                roleId = a.RoleId,
                roleName = a.Role.RoleName,
                statusId = a.StatusId,
                statusName = a.Status.StatusName,
                isActive = a.IsActive,
                isEmailVerified = a.IsEmailVerified,
                createdAt = a.CreatedAt,
                lastLoginAt = a.LastLoginAt,
                hasPassword = !string.IsNullOrWhiteSpace(a.PasswordHash),
                hasGoogleAuth = !string.IsNullOrWhiteSpace(a.GoogleId)
            })
            .OrderBy(a => a.createdAt)
            .ToListAsync(cancellationToken);

        return Ok(users);
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboard(CancellationToken cancellationToken = default)
    {
        if (!await IsAdminAsync(cancellationToken))
            return Forbid("Only administrators can access dashboard.");

        var totalUsers = await _db.Accounts.CountAsync(cancellationToken);
        var totalStudents = await _db.Accounts.CountAsync(a => a.Role.RoleName == RoleHelper.Student, cancellationToken);
        var totalInstructors = await _db.Accounts.CountAsync(a => a.Role.RoleName == RoleHelper.Instructor, cancellationToken);
        var totalAdmins = await _db.Accounts.CountAsync(a => a.Role.RoleName == RoleHelper.Admin, cancellationToken);
        var activeUsers = await _db.Accounts.CountAsync(a => a.IsActive, cancellationToken);
        var totalCourses = await _db.Courses.CountAsync(cancellationToken);
        var totalCourseRounds = await _db.CourseRounds.CountAsync(cancellationToken);
        var totalApplications = await _db.Applications.CountAsync(cancellationToken);
        var pendingApplications = await _db.Applications.CountAsync(a => a.Status.StatusName.ToLower().Contains("pending"), cancellationToken);
        var totalEnrollments = await _db.CourseRoundStudents.CountAsync(cancellationToken);

        // Recent registrations (last 30 days)
        var thirtyDaysAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        var recentRegistrations = await _db.Accounts
            .CountAsync(a => a.CreatedAt >= thirtyDaysAgo, cancellationToken);

        // Users by role
        var usersByRole = await _db.Accounts
            .Include(a => a.Role)
            .GroupBy(a => a.Role.RoleName)
            .Select(g => new { role = g.Key, count = g.Count() })
            .ToListAsync(cancellationToken);

        // Recent applications
        var recentApplications = await _db.Applications
            .Include(a => a.Course)
            .Include(a => a.Status)
            .OrderByDescending(a => a.ApplicationDate)
            .Take(10)
            .Select(a => new
            {
                id = a.Id,
                courseId = a.CourseId,
                courseTitle = a.Course.Title,
                fullName = a.FullName,
                applicationDate = a.ApplicationDate,
                statusId = a.StatusId,
                statusName = a.Status.StatusName
            })
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            summary = new
            {
                totalUsers,
                totalStudents,
                totalInstructors,
                totalAdmins,
                activeUsers,
                totalCourses,
                totalCourseRounds,
                totalApplications,
                pendingApplications,
                totalEnrollments,
                recentRegistrations
            },
            usersByRole,
            recentApplications
        });
    }

    // Helper methods
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private (bool IsValid, string ErrorMessage) ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Password is required.");

        if (password.Length < 8)
            return (false, "Password must be at least 8 characters long.");

        if (password.Length > 100)
            return (false, "Password must not exceed 100 characters.");

        if (!Regex.IsMatch(password, @"[a-z]"))
            return (false, "Password must contain at least one lowercase letter.");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            return (false, "Password must contain at least one uppercase letter.");

        if (!Regex.IsMatch(password, @"[0-9]"))
            return (false, "Password must contain at least one digit.");

        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':\\|,.<>\/?]"))
            return (false, "Password must contain at least one special character.");

        return (true, "");
    }
}
