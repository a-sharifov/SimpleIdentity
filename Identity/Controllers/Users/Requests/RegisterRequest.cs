namespace Identity.Controllers.Users.Requests;

public sealed record RegisterRequest(
    string Email,
    string Username,
    string Password,
    int RoleId,
    string ReturnUrl
    );
