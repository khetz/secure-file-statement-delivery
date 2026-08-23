using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SecureFileStatementAPI.Helpers;

public static class JwtHelper
{
    public static SecurityKey GetSigningKey(string secret)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }
}
