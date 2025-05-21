using Infrastructure.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using SevenStar.Shared.Domain.Api.Token;
using StackExchange.Redis;
using System;
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
    public static IServiceCollection AddJwtOption(this IServiceCollection services, int companyId)
    {
        services.AddSingleton<ICompanyJwtOptionsProvider, CompanyJwtOptionsProvider>();
        services.AddScoped<JwtTokenService>(serviceProvider =>
        {
            var provider = serviceProvider.GetRequiredService<ICompanyJwtOptionsProvider>();
            var jwtOption = provider.GetAsync(companyId).Result;

            return new JwtTokenService(serviceProvider, jwtOption);
        });

        return services;
    }
}
