using Common.Api.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Common.Api.Auth.Jwt;

public class JwtTokenService<TModel> : ITokenService<TModel>
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

    public string GenerateToken(TModel model)
    {
        var cfg = _configProvider.GetForModel(model);

        var claims = _claimsMapper.ToClaims(model).ToList();

        // 加入 DefaultClaims
        if (cfg.DefaultClaims != null)
        {
            foreach (var kv in cfg.DefaultClaims)
                claims.Add(new Claim(kv.Key, kv.Value));
        }
        // subject
        if (!string.IsNullOrWhiteSpace(cfg.Subject))
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, cfg.Subject));
        // jti
        if (cfg.JtiGenerator != null)
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, cfg.JtiGenerator()));
        // not before
        var nbf = cfg.NotBefore ?? DateTime.UtcNow;

        var now = DateTime.UtcNow;
        var expires = cfg.Lifetime.HasValue ? now.Add(cfg.Lifetime.Value) : (DateTime?)null;

        var securityKey = _keyProvider.GetKey(cfg.Issuer, cfg.Audience, cfg.KeyId);
        var algorithm = cfg.Algorithm ?? _keyProvider.GetAlgorithm(cfg.Issuer, cfg.Audience, cfg.KeyId);
        var creds = new SigningCredentials(securityKey, algorithm);

        var header = new JwtHeader(creds)
        {
            { "kid", cfg.KeyId }
        };

        if (!string.IsNullOrWhiteSpace(cfg.TokenType))
            header["typ"] = cfg.TokenType;

        var payload = new JwtPayload(
            issuer: cfg.Issuer,
            audience: cfg.Audience,
            claims: claims,
            notBefore: nbf,
            expires: expires,
            issuedAt: now
        );

        var token = new JwtSecurityToken(header, payload);
        return _tokenHandler.WriteToken(token);
    }

    public TModel DecrypteToken(string jwt)
    {
        var cfg = _configProvider.GetForToken(jwt);

        var key = _keyProvider.GetKey(cfg.Issuer, cfg.Audience, cfg.KeyId);
        var algorithm = cfg.Algorithm ?? _keyProvider.GetAlgorithm(cfg.Issuer, cfg.Audience, cfg.KeyId);

        var param = new TokenValidationParameters
        {
            // 動態設置
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
            ValidAudiences = cfg.ValidAudiences
        };

        var principal = _tokenHandler.ValidateToken(jwt, param, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(algorithm, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token algorithm or signature.");
        }

        return _claimsMapper.FromClaims(principal);
    }
}