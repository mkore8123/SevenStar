using Common.Api.Auth.Claims;
using Common.Api.Authen.Enum;
using Common.Api.Authen.Jwt.Implement;
using Common.Api.Authen.Jwt.Interface;
using Common.Api.Authen.Jwt.Model;
using Common.Api.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SevenStar.Shared.Domain.Api.Authen.Claims;
using SevenStar.Shared.Domain.Api.Authen.Jwt;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using System.Text.Json;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt;

/// <summary>
/// 提供 JwtTokenConfigEntity 與 JwtSigningKeyEntity 轉換為 JwtTokenConfig 的擴充方法。
/// </summary>
public static class JwtTokenConfigEntityExtensions
{
    public static IServiceCollection AddJwtTokenService(this IServiceCollection services)
    {
        services.TryAddSingleton<IApiSingletonCacheService, ApiSingletonCacheService>();
        services.AddSingleton<IJwtTokenServiceFactory<MemberClaimModel>, JwtTokenServiceFactory<MemberClaimModel>>();
        services.AddJwtTokenService<MemberClaimModel, MemberClaimMapper, DbJwtTokenConfigProvider, DbJwtSigningKeyProvider>();

        return services;
    }
}