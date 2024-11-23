using MParchin.Authority.Model;

namespace MParchin.Authority.Service;

public interface IAuthorityService
{
    public Task<DbUser> SignUpAsync(User user, string password, bool saveChanges = true);
    public Task<DbUser> SignUpOTPAsync(User user, string password, string otp, bool saveChanges = true);
    public Task<DbUser> SignInAsync(string username, string password, bool saveChanges = true);
    public Task<DbUser> SignInOTPAsync(string username, string otp, bool saveChanges = true);
    public Task<DbUser> ChangePasswordAsync(string username, string otp, string newPassword, bool saveChanges = true);
    public Task<bool> ExistsAsync(string username);
    public Task<DbUser> GetAsync(string username);
    public Task GeneratePhoneOTPAsync(string phone);
    public Task GenerateEmailOTPAsync(string email);
    public Task<bool> ConfirmOTPAsync(string username, string otp);
}