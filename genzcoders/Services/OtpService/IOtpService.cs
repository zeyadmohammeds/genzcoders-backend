namespace genzcoders.Services.OtpService;

public interface IOtpService
{
    Task<string> GenerateAndStoreOtpAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> VerifyOtpAsync(string email, string otpCode, CancellationToken cancellationToken = default);
    Task<bool> CleanupExpiredOtpsAsync(CancellationToken cancellationToken = default);
}
