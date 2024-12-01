namespace MParchin.Authority.Exceptions;

public class TokenIsNotYetValidException(string? message = null) : Exception(message ?? "Token is not yet valid")
{

}