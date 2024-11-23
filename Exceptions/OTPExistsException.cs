namespace MParchin.Authority.Exceptions;

public class OTPExistsException(string? message = null) : Exception(message ?? "OTP is already generated and sent")
{

}