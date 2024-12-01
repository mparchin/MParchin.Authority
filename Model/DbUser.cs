namespace MParchin.Authority.Model;

public partial class DbUser : User, IDbUser
{
    public int Id { get; set; }
    public string Salt { get; set; } = "";
    public string Password { get; set; } = "";

    public virtual void FillFromUser<TUser>(TUser user) where TUser : User
    {
        Guid = user.Guid;
        Name = user.Name;
        Email = user.Email;
        Phone = user.Phone;
        Role = user.Role;
        LastLogIn = user.LastLogIn;
        UpdatedAt = user.UpdatedAt;
        CreatedAt = user.CreatedAt;
    }
}