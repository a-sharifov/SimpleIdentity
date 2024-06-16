using Identity.Models;
using Identity.Jwt.Interfaces;
using Identity.Jwt.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Jwt.Services;

public class JwtManager(IOptions<JwtOptions> options) : IJwtManager
{
    private readonly JwtOptions _jwtOptions = options.Value;

    public int RefreshTokenExpirationTimeMinutes =>
        _jwtOptions.RefreshTokenExpirationTimeMinutes;

    public int TokenExpirationTimeMinutes =>
        _jwtOptions.TokenExpirationTimeMinutes;

    public string CreateTokenString(User user)
    {
        var claims = CreateClaims(user);

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

        var signingCredentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(TokenExpirationTimeMinutes),
            signingCredentials: signingCredentials
        );

        var tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return tokenValue;
    }

    private Claim[] CreateClaims(User user) =>
        [
            new Claim("id", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            ClaimValueTypes.Integer64),
            new Claim(ClaimTypes.Role, user.Role.Name)
            ];

    public string CreateRefreshTokenString()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public IEnumerable<Claim> GetClaimsInToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.ReadToken(token) is JwtSecurityToken securityToken ?
            securityToken.Claims :
            throw new SecurityTokenException("Invalid token");
    }

    public string GetEmailFromToken(string token) =>
        GetClaimsInToken(token)
        .First(x => x.Type == JwtRegisteredClaimNames.Email).Value;

    public string GetJtiFromToken(string token) =>
     GetClaimsInToken(token)
       .First(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

    public RefreshToken CreateRefreshToken()
    {
        var refreshTokenValue = CreateRefreshTokenString();
        var refreshTokenExpirationTime = DateTime.UtcNow.AddMinutes(RefreshTokenExpirationTimeMinutes);

        var token = new RefreshToken
        {
            Token = refreshTokenValue,
            ExpiredAt = refreshTokenExpirationTime
        };

        return token;
    }

    public string UpdateTokenString(string oldToken)
    {
        var claims = GetClaimsInToken(oldToken);

        var key = new SymmetricSecurityKey(
           Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

        var signingCredentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(TokenExpirationTimeMinutes),
            signingCredentials: signingCredentials
        );

        var tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return tokenValue;
    }
}