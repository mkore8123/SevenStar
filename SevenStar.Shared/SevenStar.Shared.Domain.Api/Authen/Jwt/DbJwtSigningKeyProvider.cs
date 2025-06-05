using Common.Api.Auth.Jwt;
using Microsoft.IdentityModel.Tokens;
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

    public SecurityKey GetKey(string issuer, string audience, string keyId)
    {
        

        if (!_dbJwtKeys.TryGetValue((issuer, audience, keyId), out var entry))
            throw new InvalidOperationException("找不到對應金鑰");

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(entry.Secret));
    }

    public string GetAlgorithm(string issuer, string audience, string keyId)
    {
        if (!_dbJwtKeys.TryGetValue((issuer, audience, keyId), out var entry))
            throw new InvalidOperationException("找不到對應演算法");

        return entry.Algorithm;
    }
}
