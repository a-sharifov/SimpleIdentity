using System.ComponentModel.DataAnnotations;

namespace Identity.Controllers.Users.Requests;

public sealed record LoginRequest(
    [Required] string Email, 
    [Required] string Password);
