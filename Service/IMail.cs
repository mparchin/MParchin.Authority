namespace MParchin.Authority.Service;

public interface IMail
{
    public Task SendOTPAsync(string email, string otp);
}