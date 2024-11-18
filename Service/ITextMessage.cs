namespace MParchin.Authority.Service;

public interface ITextMessage
{
    public Task SendOTP(string phone, string otp);
}