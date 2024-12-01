using MParchin.Authority.Model;

namespace MParchin.Authority.Service;

public interface IAuthorityService<TDbUser, TUser>
    where TDbUser : TUser, IDbUser, new()
    where TUser : User
{
    public Task<TDbUser> SignUpAsync(TUser user, string password, bool saveChanges = true);
    public Task<TDbUser> SignUpOTPAsync(TUser user, string password, string otp, bool saveChanges = true);
    public Task<TDbUser> SignInAsync(string username, string password, bool saveChanges = true);
    public Task<TDbUser> SignInOTPAsync(string username, string otp, bool saveChanges = true);
    public Task<TDbUser> ChangePasswordAsync(string username, string otp, string newPassword, bool saveChanges = true);
    public Task<bool> ExistsAsync(string username);
    public Task<TDbUser> GetAsync(string username);
    public Task GeneratePhoneOTPAsync(string phone);
    public Task GenerateEmailOTPAsync(string email);
    public Task<bool> ConfirmOTPAsync(string username, string otp);
}