using Common.Api.Authentication.Jwt;
using Microsoft.IdentityModel.Tokens;
using SevenStar.Shared.Domain.Database;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt;

public class JwtOptionsProvider : IJwtOptionsProvider
{
    private readonly IPlatformDb _platformDb;
    private readonly IServiceProvider _provider;

    public JwtOptionsProvider(IServiceProvider provider, IPlatformDb platformDb)
    {
        _provider = provider;
        _platformDb = platformDb;
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

    public async Task<JwtOptions> GetCompanyJwtOptionsAsync(int companyId)
    {
        if (companyId <= 0)
            throw new ArgumentOutOfRangeException(nameof(companyId));

        var companyJwtEntity = await _platformDb.GetCompanyJwtOptions(companyId);

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
    }
}