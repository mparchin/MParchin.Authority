namespace MParchin.Authority.OTP;

public interface IStorage
{
    public Task StoreAsync(string username, string otp);
    public Task<bool> ConfirmAndRemoveAsync(string username, string otp);
}