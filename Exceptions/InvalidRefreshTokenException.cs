namespace MParchin.Authority.Exceptions;

public class InvalidRefreshTokenException(string? message = null) : Exception(message ?? "Refresh token is tampered with")
{

}