using MParchin.Authority.Model;
using MParchin.Authority.Schema;

namespace MParchin.Authority.Service;

public interface IAuthorityService
{
    public Task<DbUser> SignUpAsync(User user, string password);
    public Task<DbUser> SignInAsync(string username, string password);
    public Task<DbUser> ChangePasswordAsync(DbUser user, string newPassword);
    public Task<DbUser> ChangePasswordAsync(string username, string currentPassword, string newPassword);
    public Task<bool> ExistsAsync(string username);
    public Task<DbUser> GetAsync(string username);
    public Task<DbUser> GenerateResetPasswordToken(DbUser user);
}