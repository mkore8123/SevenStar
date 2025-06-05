using Common.Api.Auth.Jwt;
using SevenStar.Shared.Domain.Api.Authen.Claim;
using SevenStar.Shared.Domain.DbContext.Platform;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using SevenStar.Shared.Domain.Service;
using System.IdentityModel.Tokens.Jwt;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt;

public class DbJwtTokenConfigProvider : IJwtTokenConfigProvider<MemberClaimModel>
{
    private readonly IPlatformDb _platformDb;
    private readonly IApiSingletonCacheService _cacheService;

    public DbJwtTokenConfigProvider(IPlatformDb platformDb, IApiSingletonCacheService cacheService)
    {
        _platformDb = platformDb;
        _cacheService = cacheService;
    }

    /// <summary>
    /// 依據模型資訊取得 JWT 設定（用於發行 Token）
    /// </summary>
    public async Task<JwtTokenConfig> GetForModelAsync(MemberClaimModel model)
    {
        var issuer = model.CompanyId;
        var audience = model.Device;

        return await _cacheService.GetOrAddJwtConfigForIssueAsync(issuer, audience, async () =>
        {
            var config = await GetLatestActiveConfigAsync(issuer, audience);

            // 挑選預設簽發金鑰
            var configKey = await GetDefaultActiveKeyAsync(config.Id);
            if (configKey == null)
                throw new InvalidOperationException("找不到對應 JWT Key 配置");

            return config.ToModel(configKey);
        });
    }

    /// <summary>
    /// 依據 Token 解構欄位取得 JWT 設定（用於驗證 Token）
    /// </summary>
    public async Task<JwtTokenConfig> GetForTokenAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var kid = jwt.Header.TryGetValue("kid", out var kidObj) ? kidObj?.ToString() ?? "" : "";
        var iss = jwt.Issuer;
        var aud = jwt.Audiences.FirstOrDefault() ?? "";

        return await _cacheService.GetOrAddJwtConfigForValidateAsync(iss, aud, kid, async () =>
        {
            var config = await GetLatestActiveConfigAsync(iss, aud);

            var configKey = await GetKeyByKidAsync(config.Id, kid);
            if (configKey == null)
                throw new InvalidOperationException("找不到對應 JWT Key 配置");

            return config.ToModel(configKey);
        });
    }

    /// <summary>
    /// 取得指定 Issuer/Audience 的最新啟用 JWT 設定
    /// </summary>
    private async Task<JwtTokenConfigEntity> GetLatestActiveConfigAsync(string issuer, string audience)
    {
        var configs = (await _platformDb.JwtTokenConfig.GetByIssuerAudienceAsync(issuer, audience))
            .Where(x => x.IsActive)
            .ToList();

        if (!configs.Any())
            throw new InvalidOperationException("找不到對應 JWT 配置");

        // 取最大版本號的現役設定
        return configs.MaxBy(x => x.VersionNo)
            ?? throw new InvalidOperationException("找不到有效 JWT 配置");
    }

    /// <summary>
    /// 取得預設啟用金鑰（未指定 kid 時用）
    /// </summary>
    private async Task<JwtSigningKeyEntity?> GetDefaultActiveKeyAsync(int configId)
    {
        var keys = (await _platformDb.JwtSigningKey.GetByConfigIdAsync(configId)).Where(x => x.IsActive).ToList();
        
        // 可以根據業務需求挑選預設金鑰，這裡直接取第一把
        return keys.FirstOrDefault();
    }

    /// <summary>
    /// 依 kid 取得金鑰（驗證 Token 用）
    /// </summary>
    private async Task<JwtSigningKeyEntity?> GetKeyByKidAsync(int configId, string kid)
    {
        var keys = (await _platformDb.JwtSigningKey.GetByConfigIdAsync(configId)).Where(x => x.IsActive).ToList();
        return keys.FirstOrDefault(x => x.KeyId == kid)
            ?? keys.FirstOrDefault(); // 若沒找到指定 kid，可考慮預設回傳第一把（視業務需求）
    }
}