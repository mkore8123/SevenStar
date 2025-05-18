using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;

namespace Common.Api.Token.Jwt;

/// <summary>
/// 自訂驗證 Jwt 處理的事件
/// </summary>
/// <typeparam name="TUserModel"></typeparam>
public class JwtBearerEventsBase : JwtBearerEvents
    // where TUserModel : class
{
    private readonly IConnectionMultiplexer _redis;
    // private readonly ILogger<CustomJwtBearerEvents<TUserModel>> _logger;

    public JwtBearerEventsBase(
        IConnectionMultiplexer redis
        /*ILogger<CustomJwtBearerEvents<TUserModel>> logger*/)
    {
        _redis = redis;
        // _logger = logger;

        OnMessageReceived = HandleMessageReceivedAsync;
        OnChallenge = HandleChallengeAsync;
        OnTokenValidated = HandleTokenValidatedAsync;
        OnAuthenticationFailed = HandleAuthFailedAsync;
        OnForbidden = HandleForbiddenAsync;
    }

    /// <summary>
    /// 第一步接收請求時，會進入這裡
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private Task HandleMessageReceivedAsync(MessageReceivedContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 第三步當 Token 驗證成功時，會進入這裡
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task HandleTokenValidatedAsync(TokenValidatedContext context)
    {
        var userId = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var tokenVersion = context.Principal?.FindFirst("token_version")?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenVersion))
        {
            // _logger.LogWarning("JWT claims 缺失，userId: {UserId}, token_version: {TokenVersion}", userId, tokenVersion);
            context.Fail("Token claims 不完整");
            return;
        }

        var redisVersion = await _redis.GetDatabase().HashGetAsync("token_versions", userId);
        if (!redisVersion.HasValue || redisVersion.ToString() != tokenVersion)
        {
            // _logger.LogWarning("Token version 不符，JWT: {TokenVersion}, Redis: {RedisVersion}", tokenVersion, redisVersion);
            context.Fail("Token 已被撤銷或版本錯誤");
            return;
        }

        // _logger.LogInformation("Token 驗證通過，UserId: {UserId}, token_version: {TokenVersion}", userId, tokenVersion);
    }

    /// <summary>
    /// 第三步當 Token 驗證失敗時，會進入這裡
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private Task HandleAuthFailedAsync(AuthenticationFailedContext context)
    {
        // _logger.LogError(context.Exception, "Token 驗證失敗");

        context.NoResult();
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync("{\"error\": \"Token 驗證失敗\"}");
    }

    /// <summary>
    /// 第二步當 Token 或未提供時，會進入這裡
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private Task HandleChallengeAsync(JwtBearerChallengeContext context)
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

    /// <summary>
    /// 最後一步，驗證通過但沒有權限時，會進入這裡
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private Task HandleForbiddenAsync(ForbiddenContext context)
    {
        // _logger.LogWarning("權限不足，403 Forbidden");

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync("{\"error\": \"禁止存取\"}");
    }
}
