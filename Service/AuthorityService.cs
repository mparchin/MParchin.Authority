using Microsoft.EntityFrameworkCore;
using MParchin.Authority.Cryptography;
using MParchin.Authority.Exceptions;
using MParchin.Authority.Model;

namespace MParchin.Authority.Service;

public class AuthorityService(IAuthorityDb db, IHash hash) : IAuthorityService
{
    public async Task<DbUser> ChangePasswordAsync(string username, string currentPassword,
        string newPassword, bool saveChanges = true) =>
        await ChangePasswordAsync(await SignInAsync(username, currentPassword, false), newPassword, saveChanges);

    public async Task<DbUser> ChangePasswordAsync(DbUser user, string newPassword, bool saveChanges = true)
    {
        hash.SetPassword(user, newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        if (saveChanges)
            await db.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ExistsAsync(string username) =>
        await db.Users.AnyAsync(user => user.Email == username || user.Password == username);

    public async Task<DbUser> GenerateResetPasswordToken(DbUser user, bool saveChanges = true)
    {
        hash.GeneratePasswordResetToken(user);
        user.UpdatedAt = DateTime.UtcNow;
        if (saveChanges)
            await db.SaveChangesAsync();
        return user;
    }

    public async Task<DbUser> GetAsync(string username) =>
        await db.Users.FirstAsync(user => user.Email == username || user.Phone == username);

    public async Task<DbUser> SignInAsync(string username, string password, bool saveChanges = true)
    {
        if (await db.Users.FirstOrDefaultAsync(user => user.Email == username || user.Phone == username) is { } dbUser)
        {
            if (hash.VerifyPassword(dbUser, password))
            {
                dbUser.LastLogIn = DateTime.UtcNow;
                if (saveChanges)
                    await db.SaveChangesAsync();
                return dbUser;
            }
        }
        throw new WrongUsernameOrPassword();
    }

    public async Task<DbUser> SignUpAsync(User user, string password, bool saveChanges = true)
    {
        if (db.Users.Any(dbUser => dbUser.Phone == user.Phone) || db.Users.Any(dbUser => dbUser.Email == user.Email))
            throw new UserExistsException();

        var dbUser = new DbUser();
        DbUser.FillGeneralDataInDb(dbUser, user);

        hash.SetPassword(dbUser, password);

        dbUser.LastLogIn = DateTime.UtcNow;
        await db.Users.AddAsync(dbUser);
        if (saveChanges)
            await db.SaveChangesAsync();

        return dbUser;
    }
}