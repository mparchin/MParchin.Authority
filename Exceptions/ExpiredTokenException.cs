namespace MParchin.Authority.Exceptions;

public class ExpiredTokenException(string? message = null) : Exception(message ?? "Token is expired")
{

}