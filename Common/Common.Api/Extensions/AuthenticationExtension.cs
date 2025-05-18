using Common.Api.Exception;
using Common.Api.Token.Jwt;
using Common.Api.Token.Jwt.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

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
