using Common.Api.Token.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


namespace Common.Api.Extensions;

/// <summary>
/// Web Api 自訂例外處理流程
/// </summary>
public static class AuthenticationExtension
{
    /// <summary>
    /// 註冊例外處理所需服務
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        // 使用 PostConfigure 來設定 JwtBearerOptions
        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, async options =>
        {
            // 注意：這裡才有 ServiceProvider 可用於注入服務
            using var provider = services.BuildServiceProvider();
            var tokenService = provider.GetRequiredService<JwtTokenServiceBase<SampleMemberModel>>();

            options.Events = tokenService.CreateJwtBearerEvents();
            options.TokenValidationParameters = tokenService.CreateValidationParameters();
        });

        return services;
    }

    /// <summary>
    /// 使用例外處理中介層
    /// </summary>
    public static IApplicationBuilder UseAuthorizationHandling(this IApplicationBuilder app)
    {
        app.UseAuthentication(); // 🔐 必須先加這個
        app.UseAuthorization();

        return app;
    }
}
