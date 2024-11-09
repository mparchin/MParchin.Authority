namespace MParchin.Authority.Exceptions;

public class InvalidTokenException(string? message = null) : Exception(message ?? "Token is tampered with")
{

}