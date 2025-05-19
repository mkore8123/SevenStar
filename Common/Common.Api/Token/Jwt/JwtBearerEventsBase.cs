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
{
    public JwtBearerEventsBase()
    {
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
    protected virtual Task HandleMessageReceivedAsync(MessageReceivedContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 第三步當 Token 驗證成功時，會進入這裡
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual Task HandleTokenValidatedAsync(TokenValidatedContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 第三步當 Token 驗證失敗時，會進入這裡
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual Task HandleAuthFailedAsync(AuthenticationFailedContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 第二步當 Token 或未提供時，會進入這裡
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual Task HandleChallengeAsync(JwtBearerChallengeContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 最後一步，驗證通過但沒有權限時，會進入這裡
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual Task HandleForbiddenAsync(ForbiddenContext context)
    {
        return Task.CompletedTask;
    }
}
