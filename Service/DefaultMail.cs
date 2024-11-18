using Microsoft.Extensions.Logging;
namespace MParchin.Authority.Service;

public class DefaultMail(ILogger<DefaultMail> logger) : IMail
{
    public Task SendOTP(string email, string otp)
    {
        logger.LogWarning("Mail service configuration not found sending otp={otp} to {email}", otp, email);
        return Task.CompletedTask;
    }
}