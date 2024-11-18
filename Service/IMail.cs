namespace MParchin.Authority.Service;

public interface IMail
{
    public Task SendOTP(string email, string otp);
}