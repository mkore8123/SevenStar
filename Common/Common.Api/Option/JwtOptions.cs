using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Option;

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
    /// 選擇的加密演算法
    /// </summary>
    public string Algorithms { get; set; } = SecurityAlgorithms.HmacSha256;

    /// <summary>
    /// Access Token 有效時間（分鐘）
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; }

    /// <summary>
    /// Refresh Token 有效時間（天）
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; }

    /// <summary>
    /// 是否啟用 TokenVersion 檢查
    /// </summary>
    public bool EnableTokenVersionCheck { get; set; } = true;
}