namespace Identity.Services.Emails.Options;

public sealed class EmailOptions
{
    public string From { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int RetryMessageSendCount { get; set; }
}
