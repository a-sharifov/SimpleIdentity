namespace Identity.Models;

public partial class BlacklistedToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiredAt { get; set; }
}
