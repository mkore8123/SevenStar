using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authen.Jwt.Model;

/// <summary>
/// JWT Token 發行與驗證的核心設定模型。
/// </summary>
public class JwtTokenConfig
{
    /// <summary>
    /// JWT 封裝類型（JWS、JWE、Nested 等）。
    /// </summary>
    // public JwtEnvelopeType Type { get; set; }

    /// <summary>
    /// Token 的發行者（iss）。
    /// 例如："https://auth.example.com"
    /// </summary>
    public string Issuer { get; set; } = default!;

    /// <summary>
    /// Token 的接收對象（aud），例如："web"、"mobile"。
    /// </summary>
    public string Audience { get; set; } = default!;

    /// <summary>
    /// Token 的有效期限，例如 60 分鐘。
    /// </summary>
    public TimeSpan? Lifetime { get; set; }

    /// <summary>
    /// 預設主體（sub claim），例如："user123"
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Token 的起始有效時間（nbf）。
    /// 可為 null 表示立即生效。
    /// </summary>
    public DateTime? NotBefore { get; set; }

    /// <summary>
    /// 是否要求 exp 存在（預設 true）。
    /// </summary>
    public bool? RequireExpirationTime { get; set; }

    /// <summary>
    /// 是否驗證 Issuer（iss）欄位。
    /// </summary>
    public bool? ValidateIssuer { get; set; }

    /// <summary>
    /// 是否驗證 Audience（aud）欄位。
    /// </summary>
    public bool? ValidateAudience { get; set; }

    /// <summary>
    /// 是否驗證 Token 是否過期。
    /// </summary>
    public bool? ValidateLifetime { get; set; }

    /// <summary>
    /// 容許的時鐘誤差，例如 5 分鐘。
    /// </summary>
    public TimeSpan? ClockSkew { get; set; }

    /// <summary>
    /// 可接受的發行者清單（如多租戶）。
    /// </summary>
    public List<string>? ValidIssuers { get; set; }

    /// <summary>
    /// 可接受的接收對象清單。
    /// </summary>
    public List<string>? ValidAudiences { get; set; }

    /// <summary>
    /// Token 類型（typ），例如 "JWT"。
    /// </summary>
    public string? TokenType { get; set; }

    /// <summary>
    /// JTI (JWT ID) 產生函式（例如 Guid）。
    /// </summary>
    public Func<string>? JtiGenerator { get; set; }

    /// <summary>
    /// 預設 Claims（補充自訂欄位）。
    /// </summary>
    public Dictionary<string, string>? DefaultClaims { get; set; }

    /// <summary>
    /// 要附加在 JWT Header 的自訂欄位。
    /// </summary>
    public Dictionary<string, object>? ExtraHeader { get; set; }

    /// <summary>
    /// 要附加在 JWT Payload 的自訂欄位。
    /// </summary>
    public Dictionary<string, object>? ExtraPayload { get; set; }
}
