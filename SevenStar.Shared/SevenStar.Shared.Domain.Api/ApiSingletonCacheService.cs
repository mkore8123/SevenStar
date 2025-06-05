using Common.Api.Auth.Jwt;
using System.Collections.Concurrent;

namespace SevenStar.Shared.Domain.Api;

public partial class ApiSingletonCacheService : IApiSingletonCacheService
{
    // 簽發時（不含 kid）
    private readonly ConcurrentDictionary<string, Lazy<Task<JwtTokenConfig>>> _jwtConfigIssueCache = new();

    // 驗證時（有含 kid）
    private readonly ConcurrentDictionary<string, Lazy<Task<JwtTokenConfig>>> _jwtConfigValidateCache = new();

    public async Task<JwtTokenConfig> GetOrAddJwtConfigForIssueAsync(string issuer, string audience, Func<Task<JwtTokenConfig>> factory)
    {
        var cacheKey = $"{issuer}::{audience}";
        var lazy = _jwtConfigIssueCache.GetOrAdd(cacheKey, _ => new Lazy<Task<JwtTokenConfig>>(factory, LazyThreadSafetyMode.ExecutionAndPublication));
        return await lazy.Value;
    }

    public async Task<JwtTokenConfig> GetOrAddJwtConfigForValidateAsync(string issuer, string audience, string kid, Func<Task<JwtTokenConfig>> factory)
    {
        var cacheKey = $"{issuer}::{audience}::{kid}";
        var lazy = _jwtConfigValidateCache.GetOrAdd(cacheKey, _ => new Lazy<Task<JwtTokenConfig>>(factory, LazyThreadSafetyMode.ExecutionAndPublication));
        return await lazy.Value;
    }
}
