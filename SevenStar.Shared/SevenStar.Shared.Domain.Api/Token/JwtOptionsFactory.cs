using Common.Api.Option;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SevenStar.Shared.Domain.Database;
using SevenStar.Shared.Domain.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Token;

public class JwtOptionsFactory : IJwtOptionsFactory
{
    private readonly IServiceProvider _provider;
    private readonly ISingletonCacheService _cacheService;

    // private readonly IPlatformDb _platformDb;

    public JwtOptionsFactory(IServiceProvider provider, ISingletonCacheService cacheService)
    {
        _provider = provider;
        _cacheService = cacheService;
    }

    public Task<JwtOptions> GetBackendJwtOptionsAsync(int backendId)
    {
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

        return Task.FromResult(jwtOptions);
    }

    public Task<JwtOptions> GetCompanyJwtOptionsAsync(int companyId)
    {
        if (companyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(companyId));

        //var entity = await _cacheService.CompnayGetOrAddAsync(companyId, () => _platformDb.GetCompanyGameDb(companyId));
        //var factory = _provider.GetRequiredKeyedService<ICompanyGameDbFactory>(entity.DataSource);

        //return await factory.CreateCompanyGameDbAsync(
        //    entity.BackendId,
        //    entity.CompanyId,
        //    entity.ConnectionString);


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

        return Task.FromResult(jwtOptions);
    }
}