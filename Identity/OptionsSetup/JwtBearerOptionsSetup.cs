using Identity.Jwt.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Api.OptionsSetup;

internal sealed class JwtBearerOptionsSetup(IConfiguration configuration)
    : IConfigureOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration = configuration;

    public void Configure(JwtBearerOptions options)
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = _configuration["Auth:Issuer"],
            RoleClaimType = ClaimTypes.Role,
            RequireExpirationTime = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!)),
        };
    }
}
