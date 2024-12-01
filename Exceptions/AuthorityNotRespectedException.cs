namespace MParchin.Authority.Exceptions;

public class AuthorityNotRespectedException(string? message = null) : Exception(message ?? "Provided issuer authority is not respected in this scope")
{

}