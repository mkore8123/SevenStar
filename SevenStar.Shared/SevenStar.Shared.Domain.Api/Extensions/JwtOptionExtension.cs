using Common.Api.Option;
using Infrastructure.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SevenStar.Shared.Domain.Api.Token;
using SevenStar.Shared.Domain.Database;
using SevenStar.Shared.Domain.DbContext;
using SevenStar.Shared.Domain.DbContext.Entity.Platform;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Extensions;

public static class JwtOptionExtension
{
    /// <summary>
    /// 註冊配置 Serilog 的設置項目，並將其設置為全域的日誌記錄器。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="config">客製化的 Serilog 配置檔案，會調用 CreateLoggerConfiguration 方法，可覆寫自行調整，傳入後會啟用</param>
    /// <returns></returns>
    public static IServiceCollection AddCompanyJwtOption(this IServiceCollection services, int companyId)
    { 
        services.AddSingleton<JwtOptions>(serviceProvider =>
        {
            var platformDb = serviceProvider.GetRequiredService<IPlatformDb>();
            var companyJwtOptionEntity = platformDb.GetCompanyJwtOptions(companyId).GetAwaiter().GetResult();

            var jwtOptions = new JwtOptions
            {
                Secret = "ThisIsAStrongSecretKeyWith32Chars!",  // ✅ 至少 32 字元，建議更複雜
                Issuer = "YourApp.Issuer",
                Audience = "YourApp.Client",
                Algorithm = SecurityAlgorithms.HmacSha256,

                AccessTokenExpirationMinutes = 60,     // 1 小時
                RefreshTokenExpirationMinutes = 43200, // 30 天

                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

                ClockSkewSeconds = 300, // 容錯時間：5 分鐘
                TokenType = "at+jwt",
                RequireExpirationTime = true
            };

            return jwtOptions;
        });

        return services;
    }
}