namespace MParchin.Authority.Exceptions;

public class WrongUsernameOrPassword(string? message = null) : Exception(message ?? "Username or password is wrong")
{

}