using Common.Api.Auth.Jwt;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt;

public class DynamicJwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMultiJwtValidationConfigProvider _configProvider;

    public DynamicJwtAuthenticationMiddleware(RequestDelegate next, IMultiJwtValidationConfigProvider configProvider)
    {
        _next = next;
        _configProvider = configProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>();
        if (allowAnonymous != null)
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var param = _configProvider.GetValidationParameters(token);
                var principal = tokenHandler.ValidateToken(token, param, out var validatedToken);
                context.User = principal;
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }
        else
        {
            var hasAuthorize = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAuthorizeData>() != null;
            if (hasAuthorize)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }

        await _next(context);
    }
}
