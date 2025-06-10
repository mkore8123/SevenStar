using Common.Api.Auth.Jwt;
using Microsoft.IdentityModel.Tokens;
using SevenStar.Shared.Domain.Api.Auth.Jwt;
using SevenStar.Shared.Domain.DbContext.Platform;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Authen.Jwt;

public class DbJwtSigningKeyProvider : IJwtSigningKeyProvider
{
    // 模擬資料庫：iss, aud, kid 唯一對應一組金鑰
    private readonly Dictionary<(string Issuer, string Audience, string Kid), (string Secret, string Algorithm)> _dbJwtKeys =
        new()
    {
        { ("companyA", "mobile", "key1"), ("CompanyA_MobileKey_012345678901234567890123456789", SecurityAlgorithms.HmacSha256) },
        { ("companyA", "web", "key2"),    ("CompanyA_WebKey_0123456789012345678901234567890", SecurityAlgorithms.HmacSha256) },
        { ("companyB", "web", "key3"),    ("CompanyB_WebKey_0123456789012345678901234567890", SecurityAlgorithms.HmacSha256) }
    };

    private readonly IPlatformDb _platformDb;
    private readonly IApiSingletonCacheService _cacheService;

    public DbJwtSigningKeyProvider(IPlatformDb platformDb, IApiSingletonCacheService cacheService)
    {
        _platformDb = platformDb;
        _cacheService = cacheService;
    }

    public async Task<SecurityKey> GetKeyAsync(string issuer, string audience, string keyId)
    {
        var configKey = await _cacheService.GetOrAddJwtConfigForValidateAsync(issuer, audience, keyId, async () =>
        {
            var tokenConfigs = await _platformDb.JwtTokenConfig.GetByIssuerAudienceAsync(issuer, audience);
            if (tokenConfigs == null || tokenConfigs.Count == 0)
            {
                return null;
            }
            var tokenConfig = tokenConfigs.Where(x => x.IsActive).MaxBy(x => x.VersionNo);
            if (tokenConfigs == null )
            {
                return null;
            }

            var configKeies = await _platformDb.JwtSigningKey.GetByConfigIdAsync(tokenConfig.Id);
            if (configKeies == null || configKeies.Count == 0)
            {
                return null;
            }

            var configKeyModel = configKeies.FirstOrDefault(key => key.KeyId == keyId);
            if (configKeyModel == null)
            {
                return null;
            }

            return tokenConfig.ToModel(configKeyModel);
        });

        if (!_dbJwtKeys.TryGetValue((issuer, audience, keyId), out var entry))
            throw new InvalidOperationException("找不到對應金鑰");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("private_key"));
        key.KeyId = keyId;
        
        return key;
    }

    public async Task<string> GetAlgorithmAsync(string issuer, string audience, string keyId)
    {
        if (!_dbJwtKeys.TryGetValue((issuer, audience, keyId), out var entry))
            throw new InvalidOperationException("找不到對應演算法");

        return entry.Algorithm;
    }
}
