using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Authentication.Jwt;

public class JwtOptions
{
    /// <summary>
    /// JWT Token 簽章金鑰（至少 32 字元）
    /// </summary>
    public string Secret { get; set; } = default!;

    /// <summary>
    /// Token 發行者
    /// </summary>
    public string Issuer { get; set; } = default!;

    /// <summary>
    /// Token 接收者（通常是前端 App 或使用者）
    /// </summary>
    public string Audience { get; set; } = default!;

    /// <summary>
    /// 選擇的加密演算法（例如：HmacSha256）
    /// </summary>
    public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;

    /// <summary>
    /// Access Token 有效時間（分鐘）
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh Token 有效時間（分鐘）
    /// </summary>
    public int RefreshTokenExpirationMinutes { get; set; } = 43200; // 30 天

    /// <summary>
    /// 是否驗證 Token 的發行者
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// 是否驗證 Token 的接收者
    /// </summary>
    public bool ValidateAudience { get; set; } = true;

    /// <summary>
    /// 是否驗證 Token 的簽章
    /// </summary>
    public bool ValidateIssuerSigningKey { get; set; } = true;

    /// <summary>
    /// 是否驗證 Token 的有效期限
    /// </summary>
    public bool ValidateLifetime { get; set; } = true;

    /// <summary>
    /// Token 過期前的容許時間差（秒）
    /// </summary>
    public int ClockSkewSeconds { get; set; } = 300;

    /// <summary>
    /// Token 的類型（例如："at+jwt"）
    /// </summary>
    public string? TokenType { get; set; }

    /// <summary>
    /// 是否要求 Token 包含過期時間
    /// </summary>
    public bool RequireExpirationTime { get; set; } = true;
}