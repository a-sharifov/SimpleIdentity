using System.IdentityModel.Tokens.Jwt;
using Identity.DbContexts;
using Identity.Models;
using Identity.Jwt.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Identity.Jwt.Services;

public sealed class JwtBlacklistManager(UserDbContext context) : IJwtBlacklistManager
{
    private readonly UserDbContext _context = context;

    public async Task RevokeAsync(string token, CancellationToken cancellationToken = default)
    {
        var tokenExpiry = GetExpiryFromToken(token);

        var blackListToken = new BlacklistedToken
        {
            Token = token,
            ExpiredAt = tokenExpiry
        };

        await _context.AddAsync(blackListToken, cancellationToken);
    }

    public async Task<bool> IsInListAsync(string token, CancellationToken cancellationToken = default) =>
        await _context.BlacklistedTokens.AnyAsync(x => x.Token == token, cancellationToken: cancellationToken);

    private static DateTime GetExpiryFromToken(string token)
    {
        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        return jwtToken?.ValidTo ?? throw new InvalidOperationException("Expiry not found in JWT token.");
    }
}
