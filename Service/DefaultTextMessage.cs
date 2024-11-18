using Microsoft.Extensions.Logging;
namespace MParchin.Authority.Service;

public class DefaultTextMessage(ILogger<DefaultTextMessage> logger) : ITextMessage
{
    public Task SendOTP(string phone, string otp)
    {
        logger.LogWarning("Text message service configuration not found sending otp={otp} to {phone}", otp, phone);
        return Task.CompletedTask;
    }
}