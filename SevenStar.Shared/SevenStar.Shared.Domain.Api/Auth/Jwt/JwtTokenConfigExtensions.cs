using Common.Api.Authen.Jwt.Implement;
using Common.Api.Authen.Jwt.Interface;
using Common.Api.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SevenStar.Shared.Domain.Api.Auth.Jwt.Provider;
using SevenStar.Shared.Domain.Api.Authen.Claims;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt;

/// <summary>
/// 提供 JwtTokenConfigEntity 與 JwtSigningKeyEntity 轉換為 JwtTokenConfig 的擴充方法。
/// </summary>
public static class JwtTokenConfigExtensions
{
    public static IServiceCollection AddJwtTokenService(this IServiceCollection services)
    {
        services.TryAddSingleton<IApiSingletonCacheService, ApiSnapCacheService>();
        services.AddSingleton<IJwtTokenServiceFactory<MemberClaimModel>, JwtTokenServiceFactory<MemberClaimModel>>();
        services.AddJwtTokenService<MemberClaimModel, MemberClaimMapper, CacheJwtTokenConfigProvider, CacheJwtSigningKeyProvider>();

        return services;
    }
}