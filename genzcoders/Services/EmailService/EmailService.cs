using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace genzcoders.Services.EmailService;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendOtpEmailAsync(string email, string otpCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 587);
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;
            var fromName = _configuration["Email:FromName"] ?? "GenzCoders";

            if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPassword))
            {
                _logger.LogWarning("Email configuration is missing. Skipping email send. In development, OTP code is: {OtpCode}", otpCode);
                return false; // Don't fail, just log in development
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Verify Your Email - GenzCoders";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2 style='color: #333;'>Email Verification</h2>
                        <p>Your verification code is:</p>
                        <h1 style='color: #007bff; font-size: 32px; letter-spacing: 5px;'>{otpCode}</h1>
                        <p>This code will expire in 10 minutes.</p>
                        <p style='color: #666; font-size: 12px;'>If you didn't request this code, please ignore this email.</p>
                    </body>
                    </html>",
                TextBody = $"Your verification code is: {otpCode}\n\nThis code will expire in 10 minutes."
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(smtpUser, smtpPassword, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("OTP email sent successfully to {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OTP email to {Email}. OTP code: {OtpCode}", email, otpCode);
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 587);
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;
            var fromName = _configuration["Email:FromName"] ?? "GenzCoders";
            // Use frontend URL for reset password links (not backend API URL)
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? _configuration["Frontend:BaseUrlHttps"] ?? "https://genzcoders.vercel.app";

            if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPassword))
            {
                _logger.LogWarning("Email configuration is missing. Password reset token: {ResetToken}", resetToken);
                return false;
            }

            // Generate frontend reset password URL with token and email as query params
            var resetUrl = $"{frontendUrl}/reset-password?token={resetToken}&email={Uri.EscapeDataString(email)}";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Password Reset Request - GenzCoders";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2 style='color: #333;'>Password Reset Request</h2>
                        <p>You requested to reset your password. Click the link below to reset it:</p>
                        <p><a href='{resetUrl}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Reset Password</a></p>
                        <p>Or copy and paste this link into your browser:</p>
                        <p style='color: #666; word-break: break-all;'>{resetUrl}</p>
                        <p style='color: #666; font-size: 12px;'>This link will expire in 1 hour.</p>
                        <p style='color: #666; font-size: 12px;'>If you didn't request a password reset, please ignore this email.</p>
                    </body>
                    </html>",
                TextBody = $"Password Reset Request\n\nClick this link to reset your password: {resetUrl}\n\nThis link will expire in 1 hour."
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(smtpUser, smtpPassword, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Password reset email sent successfully to {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
            return false;
        }
    }
}
