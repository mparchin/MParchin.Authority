namespace MParchin.Authority.Model;

public interface IDbUser
{
    public int Id { get; set; }
    public string Salt { get; set; }
    public string Password { get; set; }
    public void FillFromUser<TUser>(TUser user) where TUser : User;
}