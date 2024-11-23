using Microsoft.EntityFrameworkCore;
using MParchin.Authority.Cryptography;
using MParchin.Authority.Exceptions;
using MParchin.Authority.Model;
using MParchin.Authority.OTP;

namespace MParchin.Authority.Service;

public class AuthorityService(IAuthorityDb db, IHash hash, IMail mail, ITextMessage textMessage,
    IOTPFactory oTPFactory, IStorage storage) : IAuthorityService
{
    public async Task<DbUser> ChangePasswordAsync(string username, string otp,
        string newPassword, bool saveChanges = true)
    {
        if (!await ConfirmOTPAsync(username, otp))
            throw new WrongOTPException();
        if (!await ExistsAsync(username))
            throw new WrongUsernameOrPassword();

        var user = await GetAsync(username);
        hash.SetPassword(user, newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        if (saveChanges)
            await db.SaveChangesAsync();
        return user;
    }

    public Task<bool> ConfirmOTPAsync(string username, string otp) =>
        storage.ConfirmAndRemoveAsync(username, otp);

    public Task<bool> ExistsAsync(string username) =>
        db.Users.AnyAsync(user => user.Email == username || user.Password == username);

    public async Task GenerateEmailOTPAsync(string email)
    {
        if (await storage.ExistsAsync(email))
            throw new OTPExistsException();
        var otp = oTPFactory.Create();
        await storage.StoreAsync(email, otp);
        await mail.SendOTPAsync(email, otp);
    }

    public async Task GeneratePhoneOTPAsync(string phone)
    {
        if (await storage.ExistsAsync(phone))
            throw new OTPExistsException();
        var otp = oTPFactory.Create();
        await storage.StoreAsync(phone, otp);
        await textMessage.SendOTPAsync(phone, otp);
    }

    public Task<DbUser> GetAsync(string username) =>
        db.Users.FirstAsync(user => user.Email == username || user.Phone == username);

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

    public async Task<DbUser> SignInOTPAsync(string username, string otp, bool saveChanges = true)
    {
        if (await db.Users.FirstOrDefaultAsync(user => user.Email == username || user.Phone == username) is { } dbUser)
        {
            if (await ConfirmOTPAsync(username, otp))
            {
                dbUser.LastLogIn = DateTime.UtcNow;
                if (saveChanges)
                    await db.SaveChangesAsync();
                return dbUser;
            }
        }
        throw new WrongOTPException();
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

    public async Task<DbUser> SignUpOTPAsync(User user, string password, string otp, bool saveChanges = true) =>
        await ConfirmOTPAsync(user.Email.Contains('@') ? user.Email : user.Phone, otp)
            ? await SignUpAsync(user, password, saveChanges)
            : throw new WrongOTPException();
}