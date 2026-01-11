using genzcoders.Data;
using genzcoders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace genzcoders.Services.OtpService;

public class OtpService : IOtpService
{
    private readonly AppDbContext _db;
    private readonly ILogger<OtpService> _logger;
    private const int OtpExpirationMinutes = 10;
    private const int MaxOtpAttempts = 5;
    private static readonly Random Random = new();

    public OtpService(AppDbContext db, ILogger<OtpService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<string> GenerateAndStoreOtpAsync(string email, CancellationToken cancellationToken = default)
    {
        // Generate 6-digit OTP
        var otpCode = Random.Next(100000, 999999).ToString();

        // Invalidate any existing unused OTPs for this email
        var existingOtps = await _db.EmailOtps
            .Where(o => o.Email == email && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var otp in existingOtps)
        {
            otp.IsUsed = true;
        }

        // Create new OTP
        var emailOtp = new EmailOtp
        {
            Email = email.ToLowerInvariant(),
            OtpCode = otpCode,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpirationMinutes),
            IsUsed = false,
            Attempts = 0
        };

        _db.EmailOtps.Add(emailOtp);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("OTP generated for email: {Email}", email);
        return otpCode;
    }

    public async Task<bool> VerifyOtpAsync(string email, string otpCode, CancellationToken cancellationToken = default)
    {
        var emailLower = email.ToLowerInvariant();

        var otp = await _db.EmailOtps
            .Where(o => o.Email == emailLower && o.OtpCode == otpCode && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp == null)
        {
            // Increment attempts for any matching email OTPs (to prevent brute force)
            var recentOtps = await _db.EmailOtps
                .Where(o => o.Email == emailLower && o.CreatedAt > DateTime.UtcNow.AddMinutes(-OtpExpirationMinutes))
                .ToListAsync(cancellationToken);

            foreach (var recentOtp in recentOtps)
            {
                recentOtp.Attempts++;
            }

            await _db.SaveChangesAsync(cancellationToken);
            return false;
        }

        if (otp.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("OTP expired for email: {Email}", email);
            otp.IsUsed = true;
            await _db.SaveChangesAsync(cancellationToken);
            return false;
        }

        if (otp.Attempts >= MaxOtpAttempts)
        {
            _logger.LogWarning("OTP exceeded max attempts for email: {Email}", email);
            otp.IsUsed = true;
            await _db.SaveChangesAsync(cancellationToken);
            return false;
        }

        // Mark OTP as used
        otp.IsUsed = true;
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("OTP verified successfully for email: {Email}", email);
        return true;
    }

    public async Task<bool> CleanupExpiredOtpsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var expiredOtps = await _db.EmailOtps
                .Where(o => o.ExpiresAt < DateTime.UtcNow || o.IsUsed)
                .ToListAsync(cancellationToken);

            if (expiredOtps.Any())
            {
                _db.EmailOtps.RemoveRange(expiredOtps);
                await _db.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Cleaned up {Count} expired OTPs", expiredOtps.Count);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired OTPs");
            return false;
        }
    }
}
