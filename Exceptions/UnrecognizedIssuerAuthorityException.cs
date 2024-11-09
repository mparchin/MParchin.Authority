namespace MParchin.Authority.Exceptions;

public class UnrecognizedIssuerAuthorityException(string? message = null) : Exception(message ?? "Unrecognized Issuer Authority")
{

}