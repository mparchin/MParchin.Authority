using MParchin.Authority.Model;

namespace MParchin.Authority.Schema;

public class JWTUser : User
{
    public static Action<JWTUser, User> FillJWTFromUser { get; set; } = (jwtUser, user) =>
    {
        jwtUser.Guid = user.Guid;
        jwtUser.Name = user.Name;
        jwtUser.Email = user.Email;
        jwtUser.Phone = user.Phone;
        jwtUser.Role = user.Role;
        jwtUser.LastLogIn = user.LastLogIn;
        jwtUser.UpdatedAt = user.UpdatedAt;
    };

    public string Issuer { get; set; } = "";
    public string[] Audience { get; set; } = [];
    public DateTime Expiration { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    public JWTUser() : base() { }
    public JWTUser(User user) : base() =>
        FillJWTFromUser(this, user);
}