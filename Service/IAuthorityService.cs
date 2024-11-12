using MParchin.Authority.Model;

namespace MParchin.Authority.Service;

public interface IAuthorityService
{
    public Task<DbUser> SignUpAsync(User user, string password, bool saveChanges = true);
    public Task<DbUser> SignInAsync(string username, string password, bool saveChanges = true);
    public Task<DbUser> ChangePasswordAsync(DbUser user, string newPassword, bool saveChanges = true);
    public Task<DbUser> ChangePasswordAsync(string username, string currentPassword, string newPassword, bool saveChanges = true);
    public Task<bool> ExistsAsync(string username);
    public Task<DbUser> GetAsync(string username);
    public Task<DbUser> GenerateResetPasswordToken(DbUser user, bool saveChanges = true);
}