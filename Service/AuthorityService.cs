using Microsoft.EntityFrameworkCore;
using MParchin.Authority.Cryptography;
using MParchin.Authority.Exceptions;
using MParchin.Authority.Model;
using MParchin.Authority.Schema;

namespace MParchin.Authority.Service;

public class AuthorityService(IAuthorityDb db, IHash hash) : IAuthorityService
{
    public async Task<DbUser> ChangePasswordAsync(string username, string currentPassword, string newPassword) =>
        await ChangePasswordAsync(await SignInAsync(username, currentPassword), newPassword);

    public async Task<DbUser> ChangePasswordAsync(DbUser user, string newPassword)
    {
        hash.SetPassword(user, newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ExistsAsync(string username) =>
        await db.Users.AnyAsync(user => user.Email == username || user.Password == username);

    public async Task<DbUser> GenerateResetPasswordToken(DbUser user)
    {
        hash.GeneratePasswordResetToken(user);
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<DbUser> GetAsync(string username) =>
        await db.Users.FirstAsync(user => user.Email == username || user.Phone == username);

    public async Task<DbUser> SignInAsync(string username, string password)
    {
        if (await db.Users.FirstOrDefaultAsync(user => user.Email == username || user.Phone == username) is { } dbUser)
        {
            if (hash.VerifyPassword(dbUser, password))
            {
                dbUser.LastLogIn = DateTime.UtcNow;
                await db.SaveChangesAsync();
                return dbUser;
            }
        }
        throw new WrongUsernameOrPassword();
    }

    public async Task<DbUser> SignUpAsync(User user, string password)
    {
        if (db.Users.Any(dbUser => dbUser.Phone == user.Phone) || db.Users.Any(dbUser => dbUser.Email == user.Email))
            throw new UserExistsException();

        var dbUser = new DbUser
        {
            Email = user.Email,
            Phone = user.Phone,
            Name = user.Name,
            Role = user.Role,
        };

        hash.SetPassword(dbUser, password);

        dbUser.LastLogIn = DateTime.UtcNow;
        await db.Users.AddAsync(dbUser);
        await db.SaveChangesAsync();

        return dbUser;
    }
}