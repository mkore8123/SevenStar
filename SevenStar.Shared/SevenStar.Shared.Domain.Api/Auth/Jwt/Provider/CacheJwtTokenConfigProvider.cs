using Common.Api.Auth.Jwt.Interface.Provider;
using Common.Api.Authen.Jwt.Model;
using SevenStar.Shared.Domain.Api.Authen.Claims;
using SevenStar.Shared.Domain.DbContext.Platform;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using System.IdentityModel.Tokens.Jwt;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt.Provider;

public class CacheJwtTokenConfigProvider : IJwtTokenConfigProvider<MemberClaimModel>
{
    private readonly IApiSingletonCacheService _cacheService;

    public CacheJwtTokenConfigProvider(IPlatformDb platformDb, IApiSingletonCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    /// <summary>
    /// 依據模型資訊取得 JWT 設定（用於發行 Token）
    /// </summary>
    public async Task<JwtTokenConfig?> GetForModelAsync(MemberClaimModel model)
    {
        var issuer = model.CompanyId;
        var audience = model.Device.ToString();

        var config = _cacheService.Jwt.GetJwtConfigForIssue(issuer, audience);
        return await Task.FromResult(config);
    }

    /// <summary>
    /// 依據 Token 解構欄位取得 JWT 設定（用於驗證 Token）
    /// </summary>
    public async Task<JwtTokenConfig?> GetForTokenAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var iss = jwt.Issuer;
        var aud = jwt.Audiences.FirstOrDefault() ?? string.Empty;

        var jwtTokenConfig = _cacheService.Jwt.GetJwtConfigForIssue(iss, aud);
        
        return await Task.FromResult(jwtTokenConfig);
    }
}