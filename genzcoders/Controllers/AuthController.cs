using genzcoders.Data;
using genzcoders.DTOs.AuthDto;
using genzcoders.Helpers;
using genzcoders.Models;
using genzcoders.Services.EmailService;
using genzcoders.Services.OtpService;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace genzcoders.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<Account> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IOtpService _otpService;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<AuthController> _logger;

    private const int MaxFailedLoginAttempts = 5;
    private const int LockoutDurationMinutes = 30;
    private const int RateLimitMinutes = 15;
    private const int RateLimitMaxRequests = 5;

    public AuthController(
        AppDbContext db,
        IPasswordHasher<Account> passwordHasher,
        IConfiguration configuration,
        IEmailService emailService,
        IOtpService otpService,
        IMemoryCache memoryCache,
        ILogger<AuthController> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _emailService = emailService;
        _otpService = otpService;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    [HttpPost("send-otp")]
    public async Task<ActionResult> SendOtp([FromBody] SendOtpRequestDto dto, CancellationToken cancellationToken = default)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        // Validate email format
        if (!IsValidEmail(email))
            return BadRequest("Invalid email format.");

        // Rate limiting
        var rateLimitKey = $"otp:{email}";
        if (_memoryCache.TryGetValue(rateLimitKey, out int requestCount) && requestCount >= RateLimitMaxRequests)
        {
            return StatusCode(StatusCodes.Status429TooManyRequests, "Too many requests. Please try again later.");
        }

        // Check if email already exists
        var existingAccount = await _db.Accounts.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);

        if (existingAccount != null)
            return Conflict("Email already registered.");

        // Generate and store OTP
        var otpCode = await _otpService.GenerateAndStoreOtpAsync(email, cancellationToken);

        // Send OTP email
        await _emailService.SendOtpEmailAsync(email, otpCode, cancellationToken);

        // Set rate limit
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(RateLimitMinutes)
        };
        _memoryCache.Set(rateLimitKey, (requestCount + 1), cacheOptions);

        _logger.LogInformation("OTP sent to email: {Email}", email);
        return Ok(new { message = "OTP sent successfully. Please check your email." });
    }

    [HttpPost("verify-otp-and-register")]
    public async Task<ActionResult<AuthResponseDto>> VerifyOtpAndRegister(
        [FromBody] VerifyOtpAndRegisterDto dto,
        CancellationToken cancellationToken = default)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        // Validate password strength
        var passwordValidation = ValidatePassword(dto.Password);
        if (!passwordValidation.IsValid)
            return BadRequest(passwordValidation.ErrorMessage);

        // Verify OTP
        var isOtpValid = await _otpService.VerifyOtpAsync(email, dto.OtpCode, cancellationToken);
        if (!isOtpValid)
            return Unauthorized("Invalid or expired OTP code.");

        // Check if email already exists
        var existing = await _db.Accounts.AsNoTracking().AnyAsync(a => a.Email == email, cancellationToken);
        if (existing)
            return Conflict("Email already exists.");

        // Only allow student registration from frontend
        var roleId = await RoleHelper.GetRoleIdByNameAsync(_db, RoleHelper.Student, cancellationToken);
        if (!roleId.HasValue || roleId.Value == 0)
            return StatusCode(StatusCodes.Status500InternalServerError, "Student role not found in database. Please contact administrator.");

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
            IsEmailVerified = true,
            EmailVerifiedAt = DateTime.UtcNow,
            PasswordChangedAt = DateTime.UtcNow
        };

        account.PasswordHash = _passwordHasher.HashPassword(account, dto.Password);

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync(cancellationToken);

        // Reload with role for response
        await _db.Entry(account).Reference(a => a.Role).LoadAsync(cancellationToken);

        _logger.LogInformation("New account registered with email: {Email}", email);
        return Ok(CreateAuthResponse(account));
    }

    [HttpPost("google-auth")]
    public async Task<ActionResult<AuthResponseDto>> GoogleAuth([FromBody] GoogleAuthRequestDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var googleClientId = _configuration["Authentication:Google:ClientId"];
            if (string.IsNullOrWhiteSpace(googleClientId))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Google authentication is not configured.");
            }

            // Verify Google token
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { googleClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);

            if (payload == null)
                return Unauthorized("Invalid Google token.");

            var googleId = payload.Subject;
            var email = payload.Email?.ToLowerInvariant();
            var name = payload.Name ?? "";
            var givenName = payload.GivenName ?? "";
            var familyName = payload.FamilyName ?? "";

            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required from Google account.");

            // Check if account exists by Google ID
            var account = await _db.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.GoogleId == googleId, cancellationToken);

            if (account != null)
            {
                // Update last login
                account.LastLoginAt = DateTime.UtcNow;
                account.FailedLoginAttempts = 0;
                account.LockoutEnd = null;
                await _db.SaveChangesAsync(cancellationToken);

                // Ensure role is loaded
                if (account.Role == null)
                    await _db.Entry(account).Reference(a => a.Role).LoadAsync(cancellationToken);

                _logger.LogInformation("Google sign-in successful for existing account: {Email}", email);
                return Ok(CreateAuthResponse(account));
            }

            // Check if account exists by email (linking Google account)
            account = await _db.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);

            if (account != null)
            {
                // Link Google account
                account.GoogleId = googleId;
                account.GoogleEmail = email;
                account.LastLoginAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(cancellationToken);

                // Ensure role is loaded
                if (account.Role == null)
                    await _db.Entry(account).Reference(a => a.Role).LoadAsync(cancellationToken);

                _logger.LogInformation("Google account linked to existing account: {Email}", email);
                return Ok(CreateAuthResponse(account));
            }

            // Only allow student registration from frontend (Google auth)
            var roleId = await RoleHelper.GetRoleIdByNameAsync(_db, RoleHelper.Student, cancellationToken);
            if (!roleId.HasValue || roleId.Value == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "Student role not found in database. Please contact administrator.");

            var statusId = await _db.Statuses.AsNoTracking().OrderBy(s => s.Id).Select(s => s.Id).FirstOrDefaultAsync(cancellationToken);
            if (statusId == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, "No statuses found in database.");

            account = new Account
            {
                NationalId = "", // Will need to be updated later
                Email = email,
                FullNameEn = givenName + " " + familyName,
                FullNameAr = name,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                RoleId = roleId.Value,
                StatusId = statusId,
                IsActive = true,
                GoogleId = googleId,
                GoogleEmail = email,
                IsEmailVerified = payload.EmailVerified,
                EmailVerifiedAt = payload.EmailVerified ? DateTime.UtcNow : null,
                LastLoginAt = DateTime.UtcNow
            };

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync(cancellationToken);

            // Reload with role for response
            await _db.Entry(account).Reference(a => a.Role).LoadAsync(cancellationToken);

            _logger.LogInformation("New account created via Google: {Email}", email);
            return Ok(CreateAuthResponse(account));
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Invalid Google token provided");
            return Unauthorized("Invalid Google token.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google authentication");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during authentication.");
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Email and password are required.");

        var email = dto.Email.Trim().ToLowerInvariant();

        // Rate limiting
        var rateLimitKey = $"login:{email}";
        if (_memoryCache.TryGetValue(rateLimitKey, out int requestCount) && requestCount >= RateLimitMaxRequests)
        {
            return StatusCode(StatusCodes.Status429TooManyRequests, "Too many login attempts. Please try again later.");
        }

        var account = await _db.Accounts
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);

        if (account == null)
        {
            // Increment rate limit even for failed attempts
            IncrementRateLimit(rateLimitKey);
            return Unauthorized("Invalid credentials.");
        }

        // Check if account is locked
        if (account.LockoutEnd.HasValue && account.LockoutEnd.Value > DateTime.UtcNow)
        {
            var remainingMinutes = (int)(account.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes;
            return StatusCode(StatusCodes.Status423Locked, $"Account is locked. Try again in {remainingMinutes} minutes.");
        }

        // Check if account requires password (not Google-only)
        if (string.IsNullOrWhiteSpace(account.PasswordHash))
        {
            return BadRequest("This account uses Google authentication. Please sign in with Google.");
        }

        var verify = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, dto.Password);

        if (verify == PasswordVerificationResult.Failed)
        {
            // Increment failed login attempts
            account.FailedLoginAttempts++;
            if (account.FailedLoginAttempts >= MaxFailedLoginAttempts)
            {
                account.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
                _logger.LogWarning("Account locked due to too many failed attempts: {Email}", email);
            }
            await _db.SaveChangesAsync(cancellationToken);
            IncrementRateLimit(rateLimitKey);
            return Unauthorized("Invalid credentials.");
        }

        // Reset failed attempts and unlock account
        account.FailedLoginAttempts = 0;
        account.LockoutEnd = null;
        account.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successful login: {Email}", email);
        return Ok(CreateAuthResponse(account));
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto, CancellationToken cancellationToken = default)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        // Rate limiting
        var rateLimitKey = $"forgot-password:{email}";
        if (_memoryCache.TryGetValue(rateLimitKey, out int requestCount) && requestCount >= 3)
        {
            return StatusCode(StatusCodes.Status429TooManyRequests, "Too many requests. Please try again later.");
        }

        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email, cancellationToken);

        // Don't reveal if email exists or not (security best practice)
        if (account == null || string.IsNullOrWhiteSpace(account.PasswordHash))
        {
            IncrementRateLimit(rateLimitKey);
            // Return success even if email doesn't exist (prevent email enumeration)
            return Ok(new { message = "If the email exists, a password reset link has been sent." });
        }

        // Generate reset token
        var resetToken = GenerateSecureToken();
        account.PasswordResetToken = HashToken(resetToken);
        account.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);

        await _db.SaveChangesAsync(cancellationToken);

        // Send reset email
        await _emailService.SendPasswordResetEmailAsync(email, resetToken, cancellationToken);

        IncrementRateLimit(rateLimitKey);
        _logger.LogInformation("Password reset requested for: {Email}", email);

        return Ok(new { message = "If the email exists, a password reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto, CancellationToken cancellationToken = default)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var hashedToken = HashToken(dto.Token);

        var account = await _db.Accounts
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Email == email && a.PasswordResetToken == hashedToken);

        if (account == null)
            return Unauthorized("Invalid or expired reset token.");

        if (!account.PasswordResetTokenExpiresAt.HasValue || account.PasswordResetTokenExpiresAt.Value < DateTime.UtcNow)
        {
            account.PasswordResetToken = null;
            account.PasswordResetTokenExpiresAt = null;
            await _db.SaveChangesAsync();
            return Unauthorized("Reset token has expired.");
        }

        // Validate password strength
        var passwordValidation = ValidatePassword(dto.NewPassword);
        if (!passwordValidation.IsValid)
            return BadRequest(passwordValidation.ErrorMessage);

        // Update password
        account.PasswordHash = _passwordHasher.HashPassword(account, dto.NewPassword);
        account.PasswordResetToken = null;
        account.PasswordResetTokenExpiresAt = null;
        account.PasswordChangedAt = DateTime.UtcNow;
        account.FailedLoginAttempts = 0;
        account.LockoutEnd = null;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Password reset successfully for: {Email}", email);
        return Ok(new { message = "Password has been reset successfully." });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto, CancellationToken cancellationToken = default)
    {
        var accountIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(accountIdClaim) || !long.TryParse(accountIdClaim, out long accountId))
            return Unauthorized("Invalid token.");

        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);
        if (account == null)
            return NotFound("Account not found.");

        if (string.IsNullOrWhiteSpace(account.PasswordHash))
            return BadRequest("This account doesn't have a password set.");

        // Verify current password
        var verify = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, dto.CurrentPassword);
        if (verify == PasswordVerificationResult.Failed)
            return Unauthorized("Current password is incorrect.");

        // Validate new password strength
        var passwordValidation = ValidatePassword(dto.NewPassword);
        if (!passwordValidation.IsValid)
            return BadRequest(passwordValidation.ErrorMessage);

        // Update password
        account.PasswordHash = _passwordHasher.HashPassword(account, dto.NewPassword);
        account.PasswordChangedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password changed successfully for account: {AccountId}", accountId);
        return Ok(new { message = "Password has been changed successfully." });
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<object> Me()
    {
        var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var roleId = User.FindFirstValue("roleId");

        return Ok(new { accountId, email, roleId });
    }

    [HttpPost("register")]
    public ActionResult<AuthResponseDto> Register([FromBody] RegisterRequestDto dto)
    {
        // Legacy endpoint - redirect to OTP flow
        return BadRequest("Please use /api/auth/send-otp followed by /api/auth/verify-otp-and-register for registration with email verification.");
    }

    // Helper methods

    private AuthResponseDto CreateAuthResponse(Account account)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];
        var jwtAccessTokenMinutes = _configuration.GetValue<int?>("Jwt:AccessTokenMinutes") ?? 60;

        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException("Missing JWT signing key. Set configuration value Jwt:Key.");

        // Load role name if not already loaded
        string roleName;
        if (account.Role != null)
        {
            roleName = account.Role.RoleName;
        }
        else
        {
            var role = _db.Roles.FirstOrDefault(r => r.Id == account.RoleId);
            roleName = role?.RoleName ?? "Unknown";
        }

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(jwtAccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new(ClaimTypes.Email, account.Email),
            new("roleId", account.RoleId.ToString()),
            new("roleName", roleName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: string.IsNullOrWhiteSpace(jwtIssuer) ? null : jwtIssuer,
            audience: string.IsNullOrWhiteSpace(jwtAudience) ? null : jwtAudience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto
        {
            AccessToken = tokenString,
            ExpiresAtUtc = expires,
            AccountId = account.Id,
            Email = account.Email,
            RoleId = account.RoleId,
            RoleName = roleName
        };
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

    private string GenerateSecureToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }

    private void IncrementRateLimit(string key)
    {
        if (_memoryCache.TryGetValue(key, out int count))
        {
            _memoryCache.Set(key, count + 1, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(RateLimitMinutes)
            });
        }
        else
        {
            _memoryCache.Set(key, 1, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(RateLimitMinutes)
            });
        }
    }
}
