using Common.Api.Auth.Jwt;
using SevenStar.Shared.Domain.Api.Auth.Claims;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SevenStar.Shared.Domain.Api.Auth.Jwt;

public class DbJwtTokenConfigProvider : IJwtTokenConfigProvider<MemberClaimModel>
{
    // 模擬資料庫：iss, aud, kid 唯一對應一筆 JWT 配置
    private readonly List<JwtTokenConfig> _dbJwtOptions = new()
    {
        new JwtTokenConfig { Issuer = "companyA", Audience = "mobile", KeyId = "key1", Lifetime = TimeSpan.FromMinutes(30) },
        new JwtTokenConfig { Issuer = "companyA", Audience = "web",    KeyId = "key2", Lifetime = TimeSpan.FromMinutes(60) },
        new JwtTokenConfig { Issuer = "companyB", Audience = "web",    KeyId = "key3", Lifetime = TimeSpan.FromMinutes(45) }
    };

    public JwtTokenConfig GetForModel(MemberClaimModel model)
    {
        return _dbJwtOptions.FirstOrDefault(cfg =>
            cfg.Issuer == model.CompanyId && cfg.Audience == model.Device)
            ?? throw new InvalidOperationException("找不到對應 JWT 配置");
    }

    public JwtTokenConfig GetForToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var kid = jwt.Header.TryGetValue("kid", out var kidObj) ? kidObj?.ToString() : null;
        var iss = jwt.Issuer;
        var aud = jwt.Audiences.FirstOrDefault();

        return _dbJwtOptions.FirstOrDefault(cfg =>
            cfg.Issuer == iss && cfg.Audience == aud && cfg.KeyId == kid)
            ?? throw new InvalidOperationException("找不到對應 JWT 配置");
    }
}
