using Common.Api.Authentication;
using Common.Api.Authentication.Jwt;
using Infrastructure.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SevenStar.Shared.Domain.Api.Auth;
using SevenStar.Shared.Domain.Api.Auth.Jwt;
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
    /// 註冊配置該站點所使用的 JwtOption 參數
    /// </summary>
    /// <param name="services"></param>
    /// <param name="companyId">公司id</param>
    /// <returns></returns>
    public static IServiceCollection AddCompanyJwtOption(this IServiceCollection services, int companyId)
    {
        services.AddJwtAuthentication<JwtTokenManager, UserClaimModel>();
        services.AddScoped<IJwtOptionsProvider, JwtOptionsProvider>();
        services.AddSingleton<JwtOptions>(serviceProvider =>
        {
            var jwtOptionsFactory = serviceProvider.GetRequiredService<IJwtOptionsProvider>();
            var jwtOptions = jwtOptionsFactory.GetCompanyJwtOptionsAsync(companyId).GetAwaiter().GetResult();

            return jwtOptions;
        });

        return services;
    }
}