namespace MParchin.Authority.Service;

public interface ITextMessage
{
    public Task SendOTPAsync(string phone, string otp);
}