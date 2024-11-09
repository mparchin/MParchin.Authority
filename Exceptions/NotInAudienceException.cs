namespace MParchin.Authority.Exceptions;

public class NotInAudienceException(string? message = null) : Exception(message ?? "App is not in scope of token")
{

}