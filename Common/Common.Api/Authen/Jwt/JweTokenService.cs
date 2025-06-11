using Common.Api.Auth;
using Common.Api.Authen.Jwt.@interface;
using Common.Api.Authentication;
using Jose;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Common.Api.Authen.Jwt;

/// <summary>
/// 完全用 JOSE.JWT 實作的 JWE Token 服務
/// </summary>
public class JweTokenService<TModel> : ITokenService<TModel>
{
    private readonly IJwtTokenConfigProvider<TModel> _configProvider;
    private readonly IJweEncryptingKeyProvider _encryptingKeyProvider;
    private readonly IClaimsMapper<TModel> _claimsMapper;

    public JweTokenService(
        IJwtTokenConfigProvider<TModel> configProvider,
        IJweEncryptingKeyProvider encryptingKeyProvider,
        IClaimsMapper<TModel> claimsMapper)
    {
        _configProvider = configProvider;
        _encryptingKeyProvider = encryptingKeyProvider;
        _claimsMapper = claimsMapper;
    }

    public async Task<string> GenerateToken(TModel model)
    {
        var cfg = await _configProvider.GetForModelAsync(model);
        var now = DateTime.UtcNow;
        var nbf = cfg.NotBefore ?? now;
        var exp = cfg.Lifetime.HasValue ? now.Add(cfg.Lifetime.Value) : (DateTime?)null;

        // Claims -> Dictionary
        // 基本 claims
        var claims = _claimsMapper.ToClaimsDic(model);

        // Default claims
        if (cfg.DefaultClaims is { Count: > 0 })
            foreach (var kv in cfg.DefaultClaims)
                claims[kv.Key] = kv.Value;

        // subject
        if (!string.IsNullOrWhiteSpace(cfg.Subject))
            claims[JwtRegisteredClaimNames.Sub] = cfg.Subject;
        // jti
        if (cfg.JtiGenerator != null)
            claims[JwtRegisteredClaimNames.Jti] = cfg.JtiGenerator();

        // 標準 JWT 欄位
        claims[JwtRegisteredClaimNames.Iss] = cfg.Issuer;
        claims[JwtRegisteredClaimNames.Aud] = cfg.Audience;
        claims[JwtRegisteredClaimNames.Nbf] = new DateTimeOffset(nbf).ToUnixTimeSeconds();
        if (exp.HasValue)
            claims[JwtRegisteredClaimNames.Exp] = new DateTimeOffset(exp.Value).ToUnixTimeSeconds();

        // Extra payload
        if (cfg.ExtraPayload is { Count: > 0 })
            foreach (var kv in cfg.ExtraPayload)
                claims[kv.Key] = kv.Value;

        // 取得 JOSE 所需的 key & 演算法設定
        var key = await _encryptingKeyProvider.GetEncryptingCredentialsAsync(cfg.Issuer, cfg.Audience, cfg.JweKeyId);
        var alg = MapToJweAlgorithm(cfg.JwsSignAlgorithm!);
        var enc = MapToJweEncryption(cfg.JweEncryptAlgorithm!);

        // Header
        var headers = new Dictionary<string, object>
        {
            { "typ", "JWT" },
            { "alg", alg },
            { "enc", enc.ToString() }
        };

        if (!string.IsNullOrWhiteSpace(cfg.JweKeyId))
            headers["kid"] = cfg.JweKeyId;

        if (cfg.ExtraHeader is { Count: > 0 })
            foreach (var kv in cfg.ExtraHeader)
                headers[kv.Key] = kv.Value;

        // Jose.JWT Encode（必須同步，包進 Task.Run）
        string jwe = await Task.Run(() =>
            JWT.Encode(
                claims,
                key,
                alg,
                enc,
                extraHeaders: headers
            )
        );
        return jwe;
    }

    public async Task<TModel> DecrypteToken(string jwt)
    {
        // 先解 header，取得 iss/aud/kid
        var headers = JWT.Headers(jwt);

        string iss = headers.TryGetValue("iss", out var issObj) ? issObj?.ToString() ?? "" : "";
        string aud = headers.TryGetValue("aud", out var audObj) ? audObj?.ToString() ?? "" : "";
        string kid = headers.TryGetValue("kid", out var kidObj) ? kidObj?.ToString() ?? "" : "";

        // 你也可以自訂從 payload 取出 iss/aud/kid
        var cfg = await _configProvider.GetForTokenAsync(jwt);
        var key = await _encryptingKeyProvider.GetDecryptingKeyAsync(cfg.Issuer, cfg.Audience, cfg.JweKeyId);

        // 解密（同步，需 Task.Run 包裝）
        IDictionary<string, object> claims = await Task.Run(() =>
            JWT.Decode<IDictionary<string, object>>(jwt, key)
        );

        return _claimsMapper.FromClaimsDic(claims);
    }

    private static string MapToJweAlgorithm(JweAlgorithm alg)
    {
        return alg switch
        {
            JweAlgorithm.RSA_OAEP => "RSA-OAEP",
            JweAlgorithm.RSA_OAEP_256 => "RSA-OAEP-256",
            JweAlgorithm.RSA1_5 => "RSA1_5",
            JweAlgorithm.A128KW => "A128KW",
            JweAlgorithm.A192KW => "A192KW",
            JweAlgorithm.A256KW => "A256KW",
            JweAlgorithm.DIR => "dir",
            JweAlgorithm.ECDH_ES => "ECDH-ES",
            JweAlgorithm.ECDH_ES_A128KW => "ECDH-ES+A128KW",
            JweAlgorithm.ECDH_ES_A192KW => "ECDH-ES+A192KW",
            JweAlgorithm.ECDH_ES_A256KW => "ECDH-ES+A256KW",
            JweAlgorithm.PBES2_HS256_A128KW => "PBES2-HS256+A128KW",
            JweAlgorithm.PBES2_HS384_A192KW => "PBES2-HS384+A192KW",
            JweAlgorithm.PBES2_HS512_A256KW => "PBES2-HS512+A256KW",
            _ => throw new NotSupportedException($"不支援的 JWE 金鑰管理演算法：{alg}")
        };
    }

    public static JweAlgorithm MapToJweAlgorithm(string alg)
    {
        // 標準化：大寫、底線轉橫線
        var s = alg.Replace("_", "-").ToUpperInvariant();

        switch (s)
        {
            case var v when v == SecurityAlgorithms.RsaOaepKeyWrap.ToUpperInvariant() || v == "RSA-OAEP":
                return JweAlgorithm.RSA_OAEP;
            case "RSA-OAEP-256":
                return JweAlgorithm.RSA_OAEP_256;
            case var v when v == SecurityAlgorithms.RsaPKCS1.ToUpperInvariant() || v == "RSA1-5" || v == "RSA1_5":
                return JweAlgorithm.RSA1_5;
            case var v when v == SecurityAlgorithms.Aes128KW.ToUpperInvariant() || v == "A128KW":
                return JweAlgorithm.A128KW;
            case var v when v == SecurityAlgorithms.Aes192KW.ToUpperInvariant() || v == "A192KW":
                return JweAlgorithm.A192KW;
            case var v when v == SecurityAlgorithms.Aes256KW.ToUpperInvariant() || v == "A256KW":
                return JweAlgorithm.A256KW;
            case "DIR":
                return JweAlgorithm.DIR;
            case var v when v == SecurityAlgorithms.EcdhEs.ToUpperInvariant() || v == "ECDH-ES":
                return JweAlgorithm.ECDH_ES;
            case var v when v == SecurityAlgorithms.EcdhEsA128kw.ToUpperInvariant() || v == "ECDH-ES+A128KW":
                return JweAlgorithm.ECDH_ES_A128KW;
            case var v when v == SecurityAlgorithms.EcdhEsA192kw.ToUpperInvariant() || v == "ECDH-ES+A192KW":
                return JweAlgorithm.ECDH_ES_A192KW;
            case var v when v == SecurityAlgorithms.EcdhEsA256kw.ToUpperInvariant() || v == "ECDH-ES+A256KW":
                return JweAlgorithm.ECDH_ES_A256KW;
            case "PBES2-HS256+A128KW":
                return JweAlgorithm.PBES2_HS256_A128KW;
            case "PBES2-HS384+A192KW":
                return JweAlgorithm.PBES2_HS384_A192KW;
            case "PBES2-HS512+A256KW":
                return JweAlgorithm.PBES2_HS512_A256KW;
            default:
                throw new NotSupportedException($"不支援的 JWE 金鑰管理演算法：{alg}");
        }
    }




    private static string MapToJweEncryption(JweEncryption enc)
    {
        return enc switch
        {
            JweEncryption.A128CBC_HS256 => "A128CBC-HS256",
            JweEncryption.A192CBC_HS384 => "A192CBC-HS384",
            JweEncryption.A256CBC_HS512 => "A256CBC-HS512",
            JweEncryption.A128GCM => "A128GCM",
            JweEncryption.A192GCM => "A192GCM",
            JweEncryption.A256GCM => "A256GCM",
            _ => throw new NotSupportedException($"不支援的 JWE 內容加密演算法：{enc}")
        };
    }

    public static JweEncryption MapToJweEncryption(string enc)
    {
        // 標準化字串
        var s = enc.Replace("_", "-").ToUpperInvariant();

        switch (s)
        {
            case "A128CBC-HS256":
                return JweEncryption.A128CBC_HS256;
            case "A192CBC-HS384":
                return JweEncryption.A192CBC_HS384;
            case "A256CBC-HS512":
                return JweEncryption.A256CBC_HS512;
            case "A128GCM":
                return JweEncryption.A128GCM;
            case "A192GCM":
                return JweEncryption.A192GCM;
            case "A256GCM":
                return JweEncryption.A256GCM;
            default:
                throw new NotSupportedException($"不支援的 JWE 內容加密演算法：{enc}");
        }
    }
}

