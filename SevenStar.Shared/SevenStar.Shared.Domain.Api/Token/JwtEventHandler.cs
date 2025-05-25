using Common.Api.Token;
using Common.Api.Token.Jwt;
using Infrastructure.Caching.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;

namespace SevenStar.Shared.Domain.Api.Token;

public class JwtEventHandler : JwtBearerEventsBase
{
    private readonly IServiceProvider _provider;
    private readonly IDatabaseAsync _redisTokenDb;
    private readonly JwtTokenServiceBase<UserClaimModel> _tokenService;

    /// private readonly ILogger<JwtEventHandler> _logger;


    public JwtEventHandler(IServiceProvider provider,// [FromKeyedServices(RedisDbEnum.Token)] IDatabaseAsync redisTokenDb, 
        JwtTokenServiceBase<UserClaimModel> tokenService) 
    {
        _provider = provider;
        _tokenService = tokenService;
        _redisTokenDb = provider.GetRequiredKeyedService<IDatabaseAsync>(RedisDbEnum.Token);
    }

    protected override Task HandleMessageReceivedAsync(MessageReceivedContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 客製化處理，當驗證 token 格式正確後，去取 redis 的 refresh token 紀錄來驗證
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override Task HandleTokenValidatedAsync(TokenValidatedContext context)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleAuthFailedAsync(AuthenticationFailedContext context)
    {
        // _logger.LogError(context.Exception, "Token 驗證失敗");

        context.NoResult();
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync("{\"error\": \"Token 驗證失敗\"}");
    }

    protected override Task HandleChallengeAsync(JwtBearerChallengeContext context)
    {
        if (!context.Response.HasStarted)
        {
            // _logger.LogWarning("Token 驗證失敗或未提供，Challenge 中止預設處理");
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"請提供有效的 Token\"}");
        }

        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(ForbiddenContext context)
    {
        // _logger.LogWarning("權限不足，403 Forbidden");

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync("{\"error\": \"禁止存取\"}");
    }
}
