using Common.Api.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Common.Api.Auth.Jwt;

public class JwtTokenService<TModel> : ITokenService<TModel>, IMultiJwtValidationConfigProvider
{
    private readonly IJwtTokenConfigProvider<TModel> _configProvider;
    private readonly IJwtSigningKeyProvider _keyProvider;
    private readonly IClaimsMapper<TModel> _claimsMapper;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtTokenService(
        IJwtTokenConfigProvider<TModel> configProvider,
        IJwtSigningKeyProvider keyProvider,
        IClaimsMapper<TModel> claimsMapper)
    {
        _configProvider = configProvider;
        _keyProvider = keyProvider;
        _claimsMapper = claimsMapper;
    }

    /// <summary>
    /// 取得驗證該 token 需要的參數組合
    /// </summary>
    private (JwtTokenConfig config, SecurityKey key, string algorithm) ResolveTokenConfig(string token)
    {
        var cfg = _configProvider.GetForToken(token);
        var key = _keyProvider.GetKey(cfg.Issuer, cfg.Audience, cfg.KeyId);
        var algorithm = cfg.Algorithm ?? _keyProvider.GetAlgorithm(cfg.Issuer, cfg.Audience, cfg.KeyId);
        return (cfg, key, algorithm);
    }

    public string GenerateToken(TModel model)
    {
        var cfg = _configProvider.GetForModel(model);
        var now = DateTime.UtcNow;
        var nbf = cfg.NotBefore ?? now;
        var expires = cfg.Lifetime.HasValue ? now.Add(cfg.Lifetime.Value) : (DateTime?)null;

        // ===== 合併 Claims =====
        var claims = new List<Claim>(_claimsMapper.ToClaims(model));

        if (cfg.DefaultClaims is { Count: > 0 })
            claims.AddRange(cfg.DefaultClaims.Select(kv => new Claim(kv.Key, kv.Value)));

        if (!string.IsNullOrWhiteSpace(cfg.Subject))
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, cfg.Subject));

        if (cfg.JtiGenerator != null)
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, cfg.JtiGenerator()));

        // ===== 合併 Payload =====
        var payload = new JwtPayload(cfg.Issuer, cfg.Audience, claims, nbf, expires, now);

        if (cfg.ExtraPayload is { Count: > 0 })
            foreach (var kv in cfg.ExtraPayload)
                payload[kv.Key] = kv.Value;

        // ===== 合併 Header =====
        var securityKey = _keyProvider.GetKey(cfg.Issuer, cfg.Audience, cfg.KeyId);
        var algorithm = cfg.Algorithm ?? _keyProvider.GetAlgorithm(cfg.Issuer, cfg.Audience, cfg.KeyId);
        var creds = new SigningCredentials(securityKey, algorithm);
        var header = new JwtHeader(creds) { ["kid"] = cfg.KeyId };

        if (!string.IsNullOrWhiteSpace(cfg.TokenType))
            header["typ"] = cfg.TokenType;

        if (cfg.ExtraHeader is { Count: > 0 })
            foreach (var kv in cfg.ExtraHeader)
                header[kv.Key] = kv.Value;

        // ===== JWE: 加密模式 =====
        if (cfg.EncryptingCredentials != null)
        {
            // 注意：JWE（加密）模式下，部分自訂 Header 可能無法完全加入，受限於 CreateToken 方法。
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = cfg.Issuer,
                Audience = cfg.Audience,
                NotBefore = nbf,
                Expires = expires,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = creds,
                EncryptingCredentials = cfg.EncryptingCredentials,
                Claims = cfg.ExtraPayload // 注意：如有衝突，Claims 會覆蓋相同 key 的 claim
            };
            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }
        else
        {
            var token = new JwtSecurityToken(header, payload);
            return _tokenHandler.WriteToken(token);
        }
    }

    public TokenValidationParameters GetValidationParameters(string token)
    {
        var (cfg, key, algorithm) = ResolveTokenConfig(token);

        var param = new TokenValidationParameters
        {
            ValidIssuer = cfg.Issuer,
            ValidAudience = cfg.Audience,
            IssuerSigningKey = key,
            ValidateIssuer = cfg.ValidateIssuer ?? true,
            ValidateAudience = cfg.ValidateAudience ?? true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = cfg.ValidateLifetime ?? true,
            RequireExpirationTime = cfg.RequireExpirationTime ?? true,
            ClockSkew = cfg.ClockSkew ?? TimeSpan.FromMinutes(1),
            ValidIssuers = cfg.ValidIssuers,
            ValidAudiences = cfg.ValidAudiences,
        };

        // 若有解密金鑰，需指定
        if (cfg.EncryptingCredentials != null)
        {
            // 需配合對稱或非對稱金鑰
            param.TokenDecryptionKey = cfg.EncryptingCredentials.Key;
        }

        return param;
    }

    public TModel DecrypteToken(string jwt)
    {
        var param = GetValidationParameters(jwt);

        // 只需再抓 algorithm 做驗證
        var (_, _, algorithm) = ResolveTokenConfig(jwt);

        var principal = _tokenHandler.ValidateToken(jwt, param, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(algorithm, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token algorithm or signature.");
        }

        return _claimsMapper.FromClaims(principal);
    }
}