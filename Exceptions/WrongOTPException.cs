namespace MParchin.Authority.Exceptions;

public class WrongOTPException(string? message = null) : Exception(message ?? "Either OTP is wrong or expired")
{

}