using Common.Api.Authen.Enum;
using Common.Api.Authen.Jwt.Exception;
using Common.Api.Authen.Jwt.Interface;
using Common.Api.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Api.Authen.Claims;

/// <summary>
/// 專責處理 JWT 動態 Envelope 判斷與 claims model 綁定的 Middleware
/// 支援 JWS、JWE、Nested JWS+JWE
/// </summary>
public class DynamicJwtAuthenticationMiddleware<TClaimModel> where TClaimModel : class
{
    private readonly RequestDelegate _next;
    private readonly IJwtEnvelopeTypeResolver _envelopeTypeResolver;
    private readonly IJwtTokenServiceFactory<TClaimModel> _tokenServiceFactory;

    public DynamicJwtAuthenticationMiddleware(
        RequestDelegate next,
        IJwtEnvelopeTypeResolver envelopeTypeResolver,
        IJwtTokenServiceFactory<TClaimModel> tokenServiceFactory)
    {
        _next = next;
        _envelopeTypeResolver = envelopeTypeResolver;
        _tokenServiceFactory = tokenServiceFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            if (endpoint?.Metadata?.GetMetadata<IAuthorizeData>() != null)
            {
                await RejectUnauthorized(context);
            }
            else
            {
                await _next(context);
            }
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var envelopeType = _envelopeTypeResolver.Resolve(token);
            var tokenService = _tokenServiceFactory.Create(envelopeType, context.RequestServices);

            context.Items["CurrentClaimModel"] = await tokenService.DecrypteToken(token); 

            await _next(context);
        }
        catch (InvalidJwtException)
        {
            await RejectUnauthorized(context);
        }
        catch (Exception ex)
        {
            // 可以額外記錄日誌
            await RejectUnauthorized(context);
        }
    }

    private static async Task RejectUnauthorized(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized");
    }
}