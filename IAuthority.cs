using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using MParchin.Authority.Model;
using MParchin.Authority.Schema;

namespace MParchin.Authority;
public interface IAuthority<TJWTUser, TUser>
    where TJWTUser : JWTUser<TUser>, new()
    where TUser : User, new()
{
    public string Header { get; }
    public HashAlgorithmName HashAlgorithmName { get; }
    public RSASignaturePadding RSASignaturePadding { get; }
    public Task<TJWTUser> GetUserFromTokenAsync(string token);
    public AuthenticationTicket CreateTicket(TJWTUser user, string schemaName);
}