namespace MParchin.Authority.Schema;

public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginOTPRequest
{
    public string Username { get; set; } = "";
    public string Otp { get; set; } = "";
}