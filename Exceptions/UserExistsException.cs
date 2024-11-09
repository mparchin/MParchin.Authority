namespace MParchin.Authority.Exceptions;

public class UserExistsException(string? message = null) : Exception(message ?? "User Exists")
{

}