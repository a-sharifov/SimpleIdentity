namespace Identity.Models;

public partial class EmailConfirmationToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
