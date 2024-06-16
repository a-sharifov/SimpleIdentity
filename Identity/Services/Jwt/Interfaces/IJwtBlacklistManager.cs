namespace Identity.Jwt.Interfaces;

public interface IJwtBlacklistManager
{
    Task<bool> IsInListAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeAsync(string token, CancellationToken cancellationToken = default);
}