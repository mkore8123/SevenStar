using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Common.Api.Token.Jwt;

/// <summary>
/// 自訂 Jwt 驗證處理事件基底類別（可繼承覆寫）
/// </summary>
public class JwtBearerEventsBase : JwtBearerEvents
{
    public JwtBearerEventsBase()
    {
        InitializeEvents();
    }

    /// <summary>
    /// 註冊 JwtBearerEvents 所有事件
    /// </summary>
    protected virtual void InitializeEvents()
    {
        OnMessageReceived = HandleMessageReceivedAsync;
        OnChallenge = HandleChallengeAsync;
        OnAuthenticationFailed = HandleAuthenticationFailedAsync;
        OnTokenValidated = HandleTokenValidatedAsync;
        OnForbidden = HandleForbiddenAsync;
    }

    /// <summary>
    /// 第一步：從 HTTP 接收到訊息時觸發（通常從 Header、Query 提取 Token）
    /// </summary>
    protected virtual Task HandleMessageReceivedAsync(MessageReceivedContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 第二步：驗證失敗（如 Token 不存在、無效）時觸發（但未進入驗證流程）
    /// </summary>
    protected virtual Task HandleChallengeAsync(JwtBearerChallengeContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 第三步：驗證成功後觸發（可附加 Claims 或其他檢查）
    /// </summary>
    protected virtual Task HandleTokenValidatedAsync(TokenValidatedContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 第三步：驗證 Token 過程中發生例外時觸發（如解密失敗）
    /// </summary>
    protected virtual Task HandleAuthenticationFailedAsync(AuthenticationFailedContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 最後步驟：驗證成功但使用者沒有權限存取時觸發（如角色/權限不足）
    /// </summary>
    protected virtual Task HandleForbiddenAsync(ForbiddenContext context)
    {
        return Task.CompletedTask;
    }

    protected virtual Task HandleAuthFailedAsync(AuthenticationFailedContext context)
    {
        return Task.CompletedTask;
    }
}
