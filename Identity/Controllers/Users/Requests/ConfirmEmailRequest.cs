namespace Identity.Controllers.Users.Requests;

public record ConfirmEmailRequest(
    int UserId,
    string EmailConfirmationToken,
    string ReturnUrl
    );
