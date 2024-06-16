using System.ComponentModel.DataAnnotations;

namespace Identity.Controllers.Users.Requests;

public sealed record UpdateRefreshTokenRequest(
    [Required] string Token, 
    [Required] string RefreshToken);
