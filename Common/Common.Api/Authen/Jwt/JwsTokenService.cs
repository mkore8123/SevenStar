using Common.Api.Authen.Jwt;
using Common.Api.Authen.Jwt.Interface;
using Common.Api.Authentication;
using Jose;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Api.Auth.Jwt;

public class JwsTokenService<TModel> : ITokenService<TModel>
{
    private readonly IJwtTokenConfigProvider<TModel> _configProvider;
    private readonly IJwtSigningKeyProvider _keyProvider;
    private readonly IClaimsMapper<TModel> _claimsMapper;

    public JwsTokenService(
        IJwtTokenConfigProvider<TModel> configProvider,
        IJwtSigningKeyProvider keyProvider,
        IClaimsMapper<TModel> claimsMapper)
    {
        _configProvider = configProvider;
        _keyProvider = keyProvider;
        _claimsMapper = claimsMapper;
    }

    public async Task<string> GenerateToken(TModel model)
    {
        var cfg = await _configProvider.GetForModelAsync(model);
        var now = DateTime.UtcNow;
        var nbf = cfg.NotBefore ?? now;
        var exp = cfg.Lifetime.HasValue ? now.Add(cfg.Lifetime.Value) : (DateTime?)null;

        // 組 claims dictionary
        var claims = _claimsMapper.ToClaimsDic(model);

        if (cfg.DefaultClaims is { Count: > 0 })
            foreach (var kv in cfg.DefaultClaims)
                claims[kv.Key] = kv.Value;

        if (!string.IsNullOrWhiteSpace(cfg.Subject))
            claims[JwtRegisteredClaimNames.Sub] = cfg.Subject;

        if (cfg.JtiGenerator != null)
            claims[JwtRegisteredClaimNames.Jti] = cfg.JtiGenerator();

        // 標準欄位
        claims[JwtRegisteredClaimNames.Iss] = cfg.Issuer;
        claims[JwtRegisteredClaimNames.Aud] = cfg.Audience;
        claims[JwtRegisteredClaimNames.Nbf] = new DateTimeOffset(nbf).ToUnixTimeSeconds();
        if (exp.HasValue)
            claims[JwtRegisteredClaimNames.Exp] = new DateTimeOffset(exp.Value).ToUnixTimeSeconds();

        if (cfg.ExtraPayload is { Count: > 0 })
            foreach (var kv in cfg.ExtraPayload)
                claims[kv.Key] = kv.Value;

        // 取得金鑰 & 演算法
        var key = await _keyProvider.GetKeyAsync(cfg.Issuer, cfg.Audience, cfg.JwsKeyId);
        var joseKey = SecurityKeyToJoseKeyConverter.ToJoseKey(key);

        var algStr = cfg.JwsSignAlgorithm ?? await _keyProvider.GetAlgorithmAsync(cfg.Issuer, cfg.Audience, cfg.JwsKeyId);

        // JWS Algorithm mapping
        var alg = MapJwsAlgorithm(algStr);

        // Header
        var headers = new Dictionary<string, object>
        {
            { "typ", "JWT" },
            { "alg", alg.ToString() }
        };

        if (!string.IsNullOrWhiteSpace(cfg.JwsKeyId))
            headers["kid"] = cfg.JwsKeyId;
        if (cfg.ExtraHeader is { Count: > 0 })
            foreach (var kv in cfg.ExtraHeader)
                headers[kv.Key] = kv.Value;

        // 同步 JWT.Encode，包進 Task.Run
        string token = await Task.Run(() =>
            JWT.Encode(
                claims,
                joseKey,
                alg,
                extraHeaders: headers
            )
        );
        return token;
    }

    public async Task<TModel> DecrypteToken(string jwt)
    {
        // 拆 header 拿 kid, alg
        var headers = JWT.Headers(jwt);
        string? kid = headers.TryGetValue("kid", out var kidObj) ? kidObj?.ToString() : null;

        // 根據 config & kid 查金鑰
        var cfg = await _configProvider.GetForTokenAsync(jwt);
        var key = await _keyProvider.GetKeyAsync(cfg.Issuer, cfg.Audience, kid ?? cfg.JwsKeyId);

        // Decode/Verify（同步包進 Task.Run）
        IDictionary<string, object> claims;
        try
        {
            claims = await Task.Run(() =>
                JWT.Decode<IDictionary<string, object>>(jwt, key)
            );
        }
        catch (IntegrityException ex)
        {
            throw new SecurityTokenException("Token 簽章驗證失敗: " + ex.Message, ex);
        }
        catch (System.Exception ex)
        {
            throw new SecurityTokenException("Token 解析失敗: " + ex.Message, ex);
        }

        return _claimsMapper.FromClaimsDic(claims);
    }

    // 演算法字串 => Jose.JwsAlgorithm 映射
    private static JwsAlgorithm MapJwsAlgorithm(string alg)
    {
        // 支援常見演算法映射，可依專案需求擴充
        return alg switch
        {
            "HS256" or "HmacSha256" => JwsAlgorithm.HS256,
            "HS384" or "HmacSha384" => JwsAlgorithm.HS384,
            "HS512" or "HmacSha512" => JwsAlgorithm.HS512,
            "RS256" or "RsaSha256" => JwsAlgorithm.RS256,
            "RS384" or "RsaSha384" => JwsAlgorithm.RS384,
            "RS512" or "RsaSha512" => JwsAlgorithm.RS512,
            "ES256" or "EcdsaSha256" => JwsAlgorithm.ES256,
            "ES384" or "EcdsaSha384" => JwsAlgorithm.ES384,
            "ES512" or "EcdsaSha512" => JwsAlgorithm.ES512,
            _ => throw new NotSupportedException($"不支援的 JWT 簽章演算法：{alg}")
        };
    }

    private static string MapFromJwsAlgorithm(JwsAlgorithm alg)
    {
        return alg switch
        {
            JwsAlgorithm.HS256 => SecurityAlgorithms.HmacSha256,
            JwsAlgorithm.HS384 => SecurityAlgorithms.HmacSha384,
            JwsAlgorithm.HS512 => SecurityAlgorithms.HmacSha512,
            JwsAlgorithm.RS256 => SecurityAlgorithms.RsaSha256,
            JwsAlgorithm.RS384 => SecurityAlgorithms.RsaSha384,
            JwsAlgorithm.RS512 => SecurityAlgorithms.RsaSha512,
            JwsAlgorithm.PS256 => SecurityAlgorithms.RsaSsaPssSha256,
            JwsAlgorithm.PS384 => SecurityAlgorithms.RsaSsaPssSha384,
            JwsAlgorithm.PS512 => SecurityAlgorithms.RsaSsaPssSha512,
            JwsAlgorithm.ES256 => SecurityAlgorithms.EcdsaSha256,
            JwsAlgorithm.ES384 => SecurityAlgorithms.EcdsaSha384,
            JwsAlgorithm.ES512 => SecurityAlgorithms.EcdsaSha512,
            _ => throw new NotSupportedException($"不支援的 JWS 簽章演算法：{alg}")
        };
    }
}
