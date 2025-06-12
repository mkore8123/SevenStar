using Common.Api.Auth.Jwt;
using SevenStar.Shared.Domain.DbContext.Platform.Entity;
using System.Collections.Concurrent;

namespace SevenStar.Shared.Domain.Api;

public partial class ApiSingletonCacheService : IApiSingletonCacheService
{
    // 簽發時（不含 kid）
    private ConcurrentDictionary<string, Lazy<Task<JwtTokenConfig>>> _jwsConfigIssueCache = new();

    // 驗證時（有含 kid）
    private ConcurrentDictionary<string, Lazy<Task<JwtTokenConfig>>> _jwsConfigValidateCache = new();

    public async Task<JwtTokenConfig?> GetJwsConfigForIssueAsync(string issuer, string audience)
    {
        var cacheKey = $"{issuer}::{audience}";
        return _jwsConfigIssueCache.TryGetValue(cacheKey, out var lazy) ? await lazy.Value : null;
    }

    public async Task<JwtTokenConfig?> GetJwsConfigForValidateAsync(string issuer, string audience, string kid)
    {
        var cacheKey = $"{issuer}::{audience}::{kid}";
        return _jwsConfigValidateCache.TryGetValue(cacheKey, out var lazy) ? await lazy.Value : null;
    }

    public async Task<JwtTokenConfig> GetOrAddJwsConfigForIssueAsync(string issuer, string audience, Func<Task<JwtTokenConfig>> factory)
    {
        var cacheKey = $"{issuer}::{audience}";
        var lazy = _jwsConfigIssueCache.GetOrAdd(cacheKey, _ => new Lazy<Task<JwtTokenConfig>>(factory, LazyThreadSafetyMode.ExecutionAndPublication));
        return await lazy.Value;
    }

    public async Task<JwtTokenConfig> GetOrAddJwsConfigForValidateAsync(string issuer, string audience, string kid, Func<Task<JwtTokenConfig>> factory)
    {
        var cacheKey = $"{issuer}::{audience}::{kid}";
        var lazy = _jwsConfigValidateCache.GetOrAdd(cacheKey, _ => new Lazy<Task<JwtTokenConfig>>(factory, LazyThreadSafetyMode.ExecutionAndPublication));
        return await lazy.Value;
    }

    public bool RefreshJwtConfig(ConcurrentDictionary<string, Lazy<Task<JwtTokenConfig>>> newerJwtConfigIssue,
        ConcurrentDictionary<string, Lazy<Task<JwtTokenConfig>>> newerjwtConfigValidate)
    {
        try
        {
            // 原子性參考替換，所有新請求都會用新字典
            Interlocked.Exchange(ref _jwsConfigIssueCache, newerJwtConfigIssue);
            Interlocked.Exchange(ref _jwsConfigValidateCache, newerjwtConfigValidate);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
