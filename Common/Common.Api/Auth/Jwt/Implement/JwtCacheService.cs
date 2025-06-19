using Common.Api.Authen.Jwt.Interface;
using Common.Api.Authen.Jwt.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Jwt.Implement;

/// <summary>
/// IJwtCacheService 的實作，專責管理 JWT Token 快取存取與更新。
/// 採用 ConcurrentDictionary 儲存，支援高併發讀取。
/// </summary>
public class JwtCacheService : IJwtCacheService
{
    private ConcurrentDictionary<JwtKey, JwtTokenConfig> _jwtConfigIssueCache = new();
    private ConcurrentDictionary<JwtKey, JwtSigningKey> _jwsSigningIssueCache = new();
    private ConcurrentDictionary<JwtKey, JwtEncryptingKey> _jweEncryptingIssueCache = new();
    private ConcurrentDictionary<JwtKey, JwtSigningKey> _jwsConfigValidateCache = new();
    private ConcurrentDictionary<JwtKey, JwtEncryptingKey> _jweConfigValidateCache = new();

    public JwtTokenConfig? GetJwtConfigForIssue(string issuer, string audience)
    {
        var key = JwtKey.ForIssue(issuer, audience);
        return _jwtConfigIssueCache.TryGetValue(key, out var config) ? config : null;
    }

    public JwtSigningKey? GetJwsSigningForIssue(string issuer, string audience)
    {
        var key = JwtKey.ForIssue(issuer, audience);
        return _jwsSigningIssueCache.TryGetValue(key, out var signing) ? signing : null;
    }

    public JwtEncryptingKey? GetJweEncryptingForIssue(string issuer, string audience)
    {
        var key = JwtKey.ForIssue(issuer, audience);
        return _jweEncryptingIssueCache.TryGetValue(key, out var enc) ? enc : null;
    }

    public JwtSigningKey? GetJwsSigningForValidate(string issuer, string audience, string kid)
    {
        var key = JwtKey.ForValidate(issuer, audience, kid);
        return _jwsConfigValidateCache.TryGetValue(key, out var signing) ? signing : null;
    }

    public JwtEncryptingKey? GetJweEncryptingForValidate(string issuer, string audience, string kid)
    {
        var key = JwtKey.ForValidate(issuer, audience, kid);
        return _jweConfigValidateCache.TryGetValue(key, out var enc) ? enc : null;
    }

    public bool RefreshJwtConfig(
        ConcurrentDictionary<JwtKey, JwtTokenConfig> newerJwtConfigIssue,
        ConcurrentDictionary<JwtKey, JwtSigningKey> newerjwsConfigValidate,
        ConcurrentDictionary<JwtKey, JwtEncryptingKey> newerjweConfigValidate)
    {
        try
        {
            Interlocked.Exchange(ref _jwtConfigIssueCache, newerJwtConfigIssue);
            Interlocked.Exchange(ref _jwsConfigValidateCache, newerjwsConfigValidate);
            Interlocked.Exchange(ref _jweConfigValidateCache, newerjweConfigValidate);
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
