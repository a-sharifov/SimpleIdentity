namespace Identity.Controllers.Users.Responses;

public sealed record LoginResponse(
    string Token,
    string RefreshToken
    );