using System.Security.Claims;
using Identity.Models;

namespace Identity.Jwt.Interfaces;

public interface IJwtManager
{
    int TokenExpirationTimeMinutes { get; }
    int RefreshTokenExpirationTimeMinutes { get; }

    string CreateTokenString(User user);
    string UpdateTokenString(string token);
    string CreateRefreshTokenString();
    RefreshToken CreateRefreshToken();
    IEnumerable<Claim> GetClaimsInToken(string token);
    string GetEmailFromToken(string oldToken);
    string GetJtiFromToken(string token);
}
