using Identity.Models;
using Identity.Services.Emails.Models;

namespace Identity.Services.Emails.Interfaces;

public interface IIdentityEmailService : IEmailService
{
    Task SendConfirmationEmailAsync(SendConfirmationEmailRequest request, CancellationToken cancellationToken = default);
    Task SendConfirmationEmailAsync(User user, string returnUrl, CancellationToken cancellationToken = default);
}
