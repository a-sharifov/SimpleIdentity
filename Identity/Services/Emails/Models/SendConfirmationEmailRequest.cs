namespace Identity.Services.Emails.Models;

public sealed record SendConfirmationEmailRequest(
    string Username,
    string UserId,
    string Email,
    string EmailConfirmationToken,
    string ReturnUrl
    );
