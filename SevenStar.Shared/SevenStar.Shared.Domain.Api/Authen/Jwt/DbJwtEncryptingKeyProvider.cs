using Common.Api.Authen.Jwt;
using Common.Api.Authen.Jwt.@interface;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Authen.Jwt;

public class DbJweEncryptingKeyProvider : IJweEncryptingKeyProvider
{
    private readonly string _connStr;
    // 快取 (可依實際設計換 MemoryCache/Redis)
    private readonly ConcurrentDictionary<(string issuer, string audience, string? keyId), JwtEncryptingKey> _cache = new();

    public DbJweEncryptingKeyProvider(string connStr)
    {
        _connStr = connStr;
    }

    public async Task<object?> GetEncryptingCredentialsAsync(string issuer, string audience, string? keyId)
    {
        var key = await FindKey(issuer, audience, keyId);
        if (key == null) return null;

        var securityKey = BuildSecurityKey(key);
        return new EncryptingCredentials(
            securityKey,
            key.Algorithm,
            key.ContentAlg
        );
    }

    public async Task<SecurityKey?> GetDecryptingKeyAsync(string issuer, string audience, string? keyId)
    {
        var key = await FindKey(issuer, audience, keyId);
        if (key == null) return null;
        return BuildSecurityKey(key, decrypt: true);
    }

    public async Task<(string Algorithm, string ContentAlgorithm)?> GetAlgorithmsAsync(string issuer, string audience, string? keyId)
    {
        var key = await FindKey(issuer, audience, keyId);
        return key == null ? null : (key.Algorithm, key.ContentAlg);
    }

    /// <summary>
    /// 依據 issuer, audience, keyId 取得加密金鑰，支援快取
    /// </summary>
    private async Task<JwtEncryptingKey?> FindKey(string issuer, string audience, string? keyId)
    {
        var cacheKey = (issuer, audience, keyId);
        if (_cache.TryGetValue(cacheKey, out var cachedKey))
            return cachedKey;

        using var conn = new NpgsqlConnection(_connStr);

        // 這裡假設 config 也要查
        var sql = @"
SELECT k.*
FROM jwt_encrypting_key k
JOIN jwt_token_config c ON c.id = k.config_id
JOIN company p ON p.id = c.company_id
WHERE c.issuer = @issuer AND c.audience = @audience
  AND k.is_active AND (k.valid_to IS NULL OR k.valid_to > now())
  AND (@keyId IS NULL OR k.key_id = @keyId)
ORDER BY k.valid_from DESC
LIMIT 1";

        var key = await conn.QueryFirstOrDefaultAsync<JwtEncryptingKey>(
            sql, new { issuer, audience, keyId });

        if (key != null)
            _cache.TryAdd(cacheKey, key);

        return key;
    }

    /// <summary>
    /// 根據金鑰型態建立 SecurityKey（對稱/非對稱）
    /// </summary>
    private SecurityKey BuildSecurityKey(JwtEncryptingKey key, bool decrypt = false)
    {
        // 判斷加密演算法
        if (key.Algorithm.StartsWith("A", StringComparison.OrdinalIgnoreCase))
        {
            // AES Key Wrap 對稱加密 (A128KW, A256KW)
            var rawKey = decrypt ? key.PrivateKey : key.PublicKey ?? key.PrivateKey;
            var bytes = Convert.FromBase64String(rawKey!); // 存放時請用 base64
            return new SymmetricSecurityKey(bytes) { KeyId = key.KeyId };
        }
        else if (key.Algorithm.StartsWith("RSA", StringComparison.OrdinalIgnoreCase))
        {
            // 非對稱 RSA-OAEP
            if (decrypt && !string.IsNullOrWhiteSpace(key.PrivateKey))
            {
                // 需要私鑰解密
                var rsa = RSA.Create();
                rsa.ImportFromPem(key.PrivateKey!);
                return new RsaSecurityKey(rsa) { KeyId = key.KeyId };
            }
            else
            {
                // 用公鑰加密
                var rsa = RSA.Create();
                rsa.ImportFromPem(key.PublicKey!);
                return new RsaSecurityKey(rsa) { KeyId = key.KeyId };
            }
        }
        // 可擴充 ECDH, OctetKeyPair...
        throw new NotSupportedException($"不支援的金鑰類型: {key.Algorithm}");
    }
}