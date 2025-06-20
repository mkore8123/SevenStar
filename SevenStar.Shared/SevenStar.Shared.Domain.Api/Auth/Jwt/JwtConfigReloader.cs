using Common.Api.Authen.Jwt.Model;
using SevenStar.Shared.Domain.Api.Auth.Jwt.Mapper;
using SevenStar.Shared.Domain.DbContext.Platform;
using System.Collections.Concurrent;
using static Dapper.SqlMapper;

namespace SevenStar.Shared.Domain.Api.Authen.Jwt;

public class JwtConfigReloader
{
    private readonly IPlatformDb _platformDb;         
    private readonly IApiSingletonCacheService _cacheService;       

    public JwtConfigReloader(
        IPlatformDb platformDb,
        IApiSingletonCacheService cacheService)      // 這裡用 IApiSingletonCacheService
    {
        _platformDb = platformDb;
        _cacheService = cacheService;
    }

    /// <summary>
    /// 從資料庫拉取所有 JWT 配置，建立新的快取，並原子刷新
    /// </summary>
    public async Task<bool> ReloadAsync()
    {
        // 取得所有 jwt 設定
        var jwtTokenConfigs = await _platformDb.JwtTokenConfig.GetAllActiveAsync();
        var jwsSigningKeies = await _platformDb.JwtSigningKey.GetAllActiveAsync();
        var jweEncryptingKeies =  await _platformDb.JwtEncryptingKey.GetAllActiveAsync();

        // 建立新的快取字典
        var newJwtIssueCache = new ConcurrentDictionary<JwtKey, JwtTokenConfig>();
        var newJwsValidateCache = new ConcurrentDictionary<JwtKey, JwtSigningKey>();       
        var newJweValidateCache = new ConcurrentDictionary<JwtKey, JwtEncryptingKey>();

        var configIds = jwtTokenConfigs.Select(x => x.Id).Distinct().ToList();

        foreach (var cfgId in configIds)
        {
            var config = jwtTokenConfigs.Where(jwtTokenConfig => jwtTokenConfig.Id == cfgId).MaxBy(jwtTokenConfig => jwtTokenConfig.VersionNo);
                        
            var signingKeiesWithId = jwsSigningKeies?.Where(jwsSigningKey => jwsSigningKey.ConfigId == cfgId).ToList();
            var encryptingKeiesWithId = jweEncryptingKeies?.Where(jwsSigningKey => jwsSigningKey.ConfigId == cfgId).ToList();

            var tokenConfig = config!.ToModel()!;
            newJwtIssueCache.TryAdd(JwtKey.ForIssue(tokenConfig), tokenConfig);

            signingKeiesWithId?.ForEach(signingKey =>
            {
                var model = JwtSigningKeyMapper.ToModel(signingKey);
                var key = JwtKey.ForValidate(tokenConfig, model);
                newJwsValidateCache.TryAdd(key, model);
            });

            encryptingKeiesWithId?.ForEach(encryptingKey =>
            {
                var model = JwtEncryptingKeyMapper.ToModel(encryptingKey);
                var key = JwtKey.ForValidate(tokenConfig, model);
                newJweValidateCache.TryAdd(key, model);
            });
        }

        // 原子性快取更新
        return _cacheService.Jwt.RefreshJwtConfig(newJwtIssueCache, newJwsValidateCache, newJweValidateCache);
    }
}