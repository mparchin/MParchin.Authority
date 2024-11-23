namespace MParchin.Authority.Schema;

public class RegisterRequest
{
    public string Username { get; set; } = "";
    public string Name { get; set; } = "";
    public string Password { get; set; } = "";
}

public class RegisterOTPRequest : RegisterRequest
{
    public string Otp { get; set; } = "";
}