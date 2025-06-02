using Common.Api.Auth.Jwt;
using Common.Api.Authentication.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


namespace Common.Api.Authentication;

/// <summary>
/// Web Api 自訂例外處理流程
/// </summary>
public static class AuthenticationExtension
{
    /// <summary>
    /// 註冊 JWT 驗證所需服務
    /// </summary>
    public static IServiceCollection AddJwtAuthentication<T1, T2>(this IServiceCollection services)
        where T1 : JwtTokenServiceBase<T2>           // T1: 解析 Jwt Token 實作，必須繼承 JwtTokenServiceBase<T2>
        where T2 : class                             // T2: Token 儲存的用戶資訊模型，例如 UserClaimModel
    {
        services.AddSingleton<T1>();
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        
        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, async options =>
        {
            using var provider = services.BuildServiceProvider();
            var tokenService = provider.GetRequiredService<T1>();

            options.Events = tokenService.CreateJwtBearerEvents();
            options.TokenValidationParameters = tokenService._options.ToTokenValidationParameters();
        });

        return services;
    }

    /// <summary>
    /// 調用身分驗證流程
    /// </summary>
    public static IApplicationBuilder UseAuthorizationHandling(this IApplicationBuilder app)
    {
        app.UseAuthentication(); 
        app.UseAuthorization();

        return app;
    }
}
