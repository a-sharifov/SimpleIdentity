namespace Identity.Controllers.Users.Responses;

public sealed record UpdateRefreshTokenResponse(
    string Token,
    string RefreshToken
    );
