namespace MParchin.Authority.Model;

public partial class User
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public bool EmailVerified { get; set; }
    public string Phone { get; set; } = "";
    public bool PhoneVerified { get; set; }
    public string? Role { get; set; }
    public DateTime? LastLogIn { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}