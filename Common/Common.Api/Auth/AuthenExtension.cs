using Common.Api.Auth.Claims;
using Common.Api.Auth.Enum;
using Common.Api.Auth.Jwt;
using Common.Api.Auth.Jwt.Interface.Provider;
using Common.Api.Authen;
using Common.Api.Authen.Jwt.Enum;
using Common.Api.Authen.Jwt.Implement;
using Common.Api.Authen.Jwt.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace Common.Api.Authentication;

/// <summary>
/// Web Api 自訂例外處理流程
/// </summary>
public static class AuthenExtension
{
    /// <summary>
    /// 註冊 JWT Token 相關服務，並以 Keyed Service 方式註冊 ITokenService<TUser>（Key = TokenType.Jwt）。
    /// </summary>
    /// <typeparam name="TUser">用戶資料模型型別</typeparam>
    /// <typeparam name="TTokenConfigProvider">JWT 設定供應者型別</typeparam>
    /// <typeparam name="TSigningKeyProvider">JWT 金鑰供應者型別</typeparam>
    /// <typeparam name="TClaimsMapper">Claims 映射器型別</typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwtTokenService<TUser, TClaimsMapper, TTokenConfigProvider, TSigningKeyProvider>(this IServiceCollection services)
        where TTokenConfigProvider : class, IJwtTokenConfigProvider<TUser>
        where TSigningKeyProvider : class, IJwtSigningKeyProvider
        where TClaimsMapper : class, IClaimsMapper<TUser>
    {
        services.AddScoped<IJwtTokenConfigProvider<TUser>, TTokenConfigProvider>();
        services.AddScoped<IJwtSigningKeyProvider, TSigningKeyProvider>();
        services.AddSingleton<IClaimsMapper<TUser>, TClaimsMapper>();
        services.AddSingleton<IJwtEnvelopeTypeResolver, JwtEnvelopeTypeResolver>();
        services.AddSingleton<IJwtTokenServiceFactory<UserClaimModel>, JwtTokenServiceFactory<UserClaimModel>>();
        services.AddSingleton<IJwtCacheService, JwtSnapshotCacheService>();

        services.AddKeyedScoped<ITokenService<TUser>>(JwtEnvelopeType.Jws, (sp, key) =>
            new JwsTokenService<TUser>(
                sp.GetRequiredService<IJwtTokenConfigProvider<TUser>>(),
                sp.GetRequiredService<IJwtSigningKeyProvider>(),
                sp.GetRequiredService<IClaimsMapper<TUser>>()
            )
        );

        services.AddKeyedScoped<ITokenService<TUser>>(JwtEnvelopeType.Jwe, (sp, key) =>
            new JweTokenService<TUser>(
                sp.GetRequiredService<IJwtTokenConfigProvider<TUser>>(),
                sp.GetRequiredService<IJweEncryptingKeyProvider>(),
                sp.GetRequiredService<IClaimsMapper<TUser>>()
            )
        );

        services.AddKeyedScoped<ITokenService<TUser>>(JwtEnvelopeType.NestedJwsJwe, (sp, key) =>
            new NestedJwsJweTokenService<TUser>(
                sp.GetRequiredKeyedService<ITokenService<TUser>>(JwtEnvelopeType.Jws),
                sp.GetRequiredService<IJwtTokenConfigProvider<TUser>>(),
                sp.GetRequiredService<IJweEncryptingKeyProvider>()
            )
        );


        return services;
    }

    /// <summary>
    /// 調用身分驗證流程
    /// </summary>
    public static IApplicationBuilder UseAuthorizationHandling(this IApplicationBuilder app)
    {
        #region 因有多組 jwt token 驗證邏輯因此採用自製的 Middleware 來驗證不走標準流程
        
        // app.UseAuthentication();

        #endregion 

        app.UseAuthorization();

        return app;
    }
}
