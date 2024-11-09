using MParchin.Authority.Model;
using MParchin.Authority.Schema;

namespace MParchin.Authority.TokenFactory;

public interface IJWTFactory
{
    public JWToken Sign(User user);
    public JWToken Refresh(JWToken token);
}