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

    public Task GenerateEmailOTPAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task GeneratePhoneOTPAsync(string phone)
    {
        throw new NotImplementedException();
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

    public Task<DbUser> SignInEmailOTPAsync(string email, string otp, bool saveChanges = true)
    {
        throw new NotImplementedException();
    }

    public Task<DbUser> SignInPhoneOTPAsync(string phone, string otp, bool saveChanges = true)
    {
        throw new NotImplementedException();
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

    public Task<DbUser> SignUpPhoneEmailAsync(User user, string password, string otp, bool saveChanges = true)
    {
        throw new NotImplementedException();
    }

    public Task<DbUser> SignUpPhoneOTPAsync(User user, string password, string otp, bool saveChanges = true)
    {
        throw new NotImplementedException();
    }
}