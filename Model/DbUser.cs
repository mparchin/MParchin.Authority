using System.ComponentModel.DataAnnotations.Schema;

namespace MParchin.Authority.Model;

public partial class DbUser : User
{
    [NotMapped]
    public static Action<DbUser, User> FillGeneralDataInDb { get; set; } = (dbUser, user) =>
    {
        dbUser.Email = user.Email;
        dbUser.Phone = user.Phone;
        dbUser.Name = user.Name;
        dbUser.Role = user.Role;
        dbUser.EmailVerified = user.EmailVerified;
        dbUser.PhoneVerified = user.PhoneVerified;
    };

    public int Id { get; set; }
    public string Salt { get; set; } = "";
    public string Password { get; set; } = "";
    public string ResetToken { get; set; } = "";
    public DateTime ResetExpirationTime { get; set; } = DateTime.MinValue;
}