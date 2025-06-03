using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Auth.Jwt;

public class JwtTokenConfig
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string KeyId { get; set; } = default!;
    public TimeSpan? Lifetime { get; set; }

    public string? Algorithm { get; set; }               // 預設 HmacSha256
    public bool? RequireExpirationTime { get; set; }
    public bool? ValidateIssuer { get; set; }
    public bool? ValidateAudience { get; set; }
    public bool? ValidateLifetime { get; set; }
    public TimeSpan? ClockSkew { get; set; }

    public List<string>? ValidIssuers { get; set; }
    public List<string>? ValidAudiences { get; set; }
    public string? Subject { get; set; }
    public DateTime? NotBefore { get; set; }
    public Dictionary<string, string>? DefaultClaims { get; set; }
    public string? TokenType { get; set; }
    public Func<string>? JtiGenerator { get; set; }
    // ...甚至可以有 EncryptingCredentials、ExtraHeader、ExtraPayload 等
}