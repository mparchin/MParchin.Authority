namespace MParchin.Authority.Schema;

public class ChangePasswordRequest
{
    public string NewPassword { get; set; } = "";
    public string Otp { get; set; } = "";
}