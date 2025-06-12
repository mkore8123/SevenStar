using Common.Api.Auth.Jwt;
using Common.Api.Authen.Enum;
using SevenStar.Shared.Domain.Api.Auth.Jwt;
using SevenStar.Shared.Domain.DbContext.Platform;
using SevenStar.Shared.Domain.DbContext.Platform.Repository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        var newIssueCache = new ConcurrentDictionary<string, Lazy<Task<JwtTokenConfig>>>();
        var newValidateCache = new ConcurrentDictionary<string, Lazy<Task<JwtTokenConfig>>>();

        var configIds = jwtTokenConfigs.Select(x => x.Id).Distinct().ToList();

        if (configIds == null || configIds.Count == 0)
        {
            return false;
        }

        if (jwsSigningKeies == null || jwsSigningKeies.Count == 0)
        {
            return false;
        }

        foreach (var cfgId in configIds)
        {
            var type = JwtEnvelopeType.PlainJwt;
            var config = jwtTokenConfigs.Where(jwtTokenConfig => jwtTokenConfig.Id == cfgId).MaxBy(jwtTokenConfig => jwtTokenConfig.VersionNo);
            
            var signingKeiesWithId = jwsSigningKeies.Where(jwsSigningKey => jwsSigningKey.ConfigId == cfgId).ToList();
            var signingKeySelector = signingKeiesWithId?.MaxBy(jwsSigningKey => jwsSigningKey.ValidTo);

            var encryptKeiesWithId = jweEncryptingKeies?.Where(jwsSigningKey => jwsSigningKey.ConfigId == cfgId).ToList();
            var encryptKeySelector = encryptKeiesWithId?.MaxBy(jwsSigningKey => jwsSigningKey.ValidTo);

            if (signingKeySelector != null)
            {
                type = JwtEnvelopeType.Jws;
            }

            if (encryptKeySelector != null)
            {
                type = signingKeySelector == null ? JwtEnvelopeType.Jwe : JwtEnvelopeType.NestedJwsJwe;
            }

            var tokenConfig = config!.ToModel(signingKeySelector, encryptKeySelector);
            newIssueCache.TryAdd($"{config!.Issuer}::{config!.Audience}", new Lazy<Task<JwtTokenConfig>>(() => Task.FromResult(tokenConfig)));

            foreach(var signingKey in signingKeiesWithId)
            {
                var tokenConfig2 = config!.ToModel(signingKey, encryptKeySelector);
                newValidateCache.TryAdd($"{config.Issuer}::{config.Audience}::{signingKey.KeyId}", new Lazy<Task<JwtTokenConfig>>(() => Task.FromResult(tokenConfig2)));
            }
        }

        // 原子性快取更新
        return _cacheService.RefreshJwtConfig(newIssueCache, newValidateCache);
    }
}