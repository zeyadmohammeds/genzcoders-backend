using genzcoders.Data;
using genzcoders.DTOs.AuthDto;
using genzcoders.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace genzcoders.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(AppDbContext db, ILogger<ProfileController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<UserProfileDto>> GetProfile(CancellationToken cancellationToken = default)
    {
        var accountIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(accountIdClaim) || !long.TryParse(accountIdClaim, out long accountId))
            return Unauthorized("Invalid token.");

        var account = await _db.Accounts
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        if (account == null)
            return NotFound("Account not found.");

        var profile = new UserProfileDto
        {
            Id = account.Id,
            Email = account.Email,
            Phone = account.Phone,
            NationalId = account.NationalId,
            FullNameEn = account.FullNameEn,
            FullNameAr = account.FullNameAr,
            RoleId = account.RoleId,
            RoleName = account.Role.RoleName,
            CreatedAt = account.CreatedAt,
            IsActive = account.IsActive,
            IsEmailVerified = account.IsEmailVerified,
            EmailVerifiedAt = account.EmailVerifiedAt,
            LastLoginAt = account.LastLoginAt,
            HasPassword = !string.IsNullOrWhiteSpace(account.PasswordHash),
            HasGoogleAuth = !string.IsNullOrWhiteSpace(account.GoogleId)
        };

        return Ok(profile);
    }

    [HttpPut]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileDto dto, CancellationToken cancellationToken = default)
    {
        var accountIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(accountIdClaim) || !long.TryParse(accountIdClaim, out long accountId))
            return Unauthorized("Invalid token.");

        var account = await _db.Accounts
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        if (account == null)
            return NotFound("Account not found.");

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            
            // Validate email format
            if (!IsValidEmail(email))
                return BadRequest("Invalid email format.");

            // Check if email is already taken by another account
            var emailExists = await _db.Accounts
                .AnyAsync(a => a.Email == email && a.Id != accountId, cancellationToken);
            
            if (emailExists)
                return Conflict("Email is already in use by another account.");

            account.Email = email;
            // If email changed, mark as unverified
            if (account.Email != email)
            {
                account.IsEmailVerified = false;
                account.EmailVerifiedAt = null;
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.FullNameEn))
            account.FullNameEn = dto.FullNameEn;

        if (!string.IsNullOrWhiteSpace(dto.FullNameAr))
            account.FullNameAr = dto.FullNameAr;

        if (dto.Phone != null)
            account.Phone = dto.Phone;

        if (!string.IsNullOrWhiteSpace(dto.NationalId))
        {
            // Check if national ID is already taken by another account
            var nationalIdExists = await _db.Accounts
                .AnyAsync(a => a.NationalId == dto.NationalId && a.Id != accountId, cancellationToken);
            
            if (nationalIdExists)
                return Conflict("National ID is already in use by another account.");

            account.NationalId = dto.NationalId;
        }

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile updated for account: {AccountId}", accountId);

        var updatedProfile = new UserProfileDto
        {
            Id = account.Id,
            Email = account.Email,
            Phone = account.Phone,
            NationalId = account.NationalId,
            FullNameEn = account.FullNameEn,
            FullNameAr = account.FullNameAr,
            RoleId = account.RoleId,
            RoleName = account.Role.RoleName,
            CreatedAt = account.CreatedAt,
            IsActive = account.IsActive,
            IsEmailVerified = account.IsEmailVerified,
            EmailVerifiedAt = account.EmailVerifiedAt,
            LastLoginAt = account.LastLoginAt,
            HasPassword = !string.IsNullOrWhiteSpace(account.PasswordHash),
            HasGoogleAuth = !string.IsNullOrWhiteSpace(account.GoogleId)
        };

        return Ok(updatedProfile);
    }

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
}
