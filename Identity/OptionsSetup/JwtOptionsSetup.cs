using Identity.Jwt.Options;
using Microsoft.Extensions.Options;

namespace Api.OptionsSetup;

internal sealed class JwtOptionsSetup(IConfiguration configuration)
    : IConfigureOptions<JwtOptions>
{
    private readonly IConfiguration _configuration = configuration;

    public void Configure(JwtOptions options)
    {
        _configuration.GetSection(SD.JwtSectionKey).Bind(options);
        options.Issuer = _configuration["Auth:Issuer"]!;
    }
}
