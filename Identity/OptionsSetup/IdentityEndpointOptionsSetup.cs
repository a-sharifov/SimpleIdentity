using Identity.Services.Endpoints.Options;
using Microsoft.Extensions.Options;

namespace Api.OptionsSetup;

public class IdentityEndpointOptionsSetup(IConfiguration configuration)
    : IConfigureOptions<IdentityEndpointOptions>
{
    private readonly IConfiguration _configuration = configuration;

    public void Configure(IdentityEndpointOptions options) => 
        options.BaseUrl = _configuration["Auth:Identity"]!;
}
    