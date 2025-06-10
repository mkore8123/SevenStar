using Common.Api.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

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
    private async Task<(JwtTokenConfig config, SecurityKey key, string algorithm)> ResolveTokenConfig(string token)
    {
        var cfg = await _configProvider.GetForTokenAsync(token);
        var key = await _keyProvider.GetKeyAsync(cfg.Issuer, cfg.Audience, cfg.KeyId);
        var algorithm = cfg.Algorithm ?? await _keyProvider.GetAlgorithmAsync(cfg.Issuer, cfg.Audience, cfg.KeyId);
        return (cfg, key, algorithm);
    }

    public async Task<string> GenerateToken(TModel model)
    {
        var cfg = await _configProvider.GetForModelAsync(model);
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

        // ===== 合併 Header - 1 =====
        var securityKey = await _keyProvider.GetKeyAsync(cfg.Issuer, cfg.Audience, cfg.KeyId);
        var algorithm = cfg.Algorithm ?? await _keyProvider.GetAlgorithmAsync(cfg.Issuer, cfg.Audience, cfg.KeyId);
        var creds = new SigningCredentials(securityKey, algorithm);

        // ===== JWE: 加密模式 =====
        if (cfg.EncryptingCredentials != null)
        {
            // 注意：JWE（加密）模式下，部分自訂 Header 可能無法完全加入，受限於 CreateToken 方法。
            // 不需要帶入自訂 Header; 會由 SecurityTokenDescriptor 底層自動生成 JWE 所需的基本 Header。
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = cfg.Issuer,
                Audience = cfg.Audience,
                NotBefore = nbf,
                Expires = expires,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = creds,
                EncryptingCredentials = cfg.EncryptingCredentials,
                Claims = cfg.ExtraPayload, // 注意：如有衝突，Subject 的 Claim 會覆蓋 Claims 相同 key 的值
                AdditionalHeaderClaims = cfg.ExtraHeader 
            };
            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }
        else
        {
            // ===== 合併 Header - 2 =====
            // 基本的 JWT 簽章可以自訂 JWT Header
            var header = new JwtHeader(creds) { ["kid"] = cfg.KeyId };

            if (!string.IsNullOrWhiteSpace(cfg.TokenType))
                header["typ"] = cfg.TokenType;

            if (cfg.ExtraHeader is { Count: > 0 })
                foreach (var kv in cfg.ExtraHeader)
                    header[kv.Key] = kv.Value;

            var token = new JwtSecurityToken(header, payload);
            return _tokenHandler.WriteToken(token);
        }
    }

    public async Task<TokenValidationParameters> GetValidationParameters(string token)
    {
        var (cfg, key, algorithm) = await ResolveTokenConfig(token);

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

    public async Task<TModel> DecrypteToken(string jwt)
    {
        var param = await GetValidationParameters(jwt);

        // 只需再抓 algorithm 做驗證
        var (_, _, algorithm) = await ResolveTokenConfig(jwt);

        var principal = _tokenHandler.ValidateToken(jwt, param, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(algorithm, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token algorithm or signature.");
        }

        return _claimsMapper.FromClaims(principal);
    }
}