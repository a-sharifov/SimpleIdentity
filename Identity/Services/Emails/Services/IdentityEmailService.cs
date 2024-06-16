using Identity.Models;
using Identity.Services.Emails;
using Identity.Services.Emails.Interfaces;
using Identity.Services.Emails.Models;
using Identity.Services.Emails.Options;
using Identity.Services.Emails.Services;
using Identity.Services.Endpoints.Options;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace Identity.Emails.Services;

public sealed class IdentityEmailService
    (IOptions<EmailOptions> options,
     IOptions<IdentityEndpointOptions> identityEndpointOptions) :
    EmailBaseService(options), 
    IIdentityEmailService
{
    private readonly IdentityEndpointOptions _identityEndpointOptions = identityEndpointOptions.Value;

    public async Task SendConfirmationEmailAsync(SendConfirmationEmailRequest request, CancellationToken cancellationToken = default)
    {
        var confirmEmailTemplatePath = EmailTemplatePath.ConfirmEmailTemplate;

        string confirmEmailTemplate =
            await File.ReadAllTextAsync(confirmEmailTemplatePath, cancellationToken);

        var confirmUrl =
           $@"{_identityEndpointOptions.BaseUrl}/api/users/confirm-email?userId={request.UserId}&emailConfirmationToken={request.EmailConfirmationToken}&returnUrl={request.ReturnUrl}";

        var confirmUrlEncode = HtmlEncoder.Default.Encode(confirmUrl);

        confirmEmailTemplate =
            confirmEmailTemplate
            .Replace("{{username}}", request.Username)
            .Replace("{{confirmationLink}}", confirmUrlEncode);

        var sendMessageRequest = new SendMessageRequest(
            To: request.Email,
            Subject: $"Task - confirm email",
            Body: confirmEmailTemplate
            );

        await SendMessageAsync(sendMessageRequest, cancellationToken);
    }

    public async Task SendConfirmationEmailAsync(User user, string returnUrl, CancellationToken cancellationToken = default)
    {
        var request =
            new SendConfirmationEmailRequest(
                user.Username,
                user.Id.ToString(),
                user.Email,
                user.EmailConfirmationToken!.Token,
                returnUrl);

        await SendConfirmationEmailAsync(request, cancellationToken);
    }
}
