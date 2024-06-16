using Identity.Jwt.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Middlewares;

public sealed class JwtBlackListMiddleware(
    RequestDelegate next,
    IJwtBlacklistManager blacklistManager)
{
    private readonly RequestDelegate _next = next;
    private readonly IJwtBlacklistManager _blacklistManager = blacklistManager;

    public async Task InvokeAsync(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers.Authorization.FirstOrDefault();
        var isInBlacklist = authorizationHeader != null
            && await _blacklistManager.IsInListAsync(authorizationHeader["Bearer ".Length..]);

        if (!isInBlacklist)
        {
            await _next(context);
            return;
        }

      
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }

}
