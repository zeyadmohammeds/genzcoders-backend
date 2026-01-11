namespace genzcoders.Services.EmailService;

public interface IEmailService
{
    Task<bool> SendOtpEmailAsync(string email, string otpCode, CancellationToken cancellationToken = default);
    Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken = default);
}
