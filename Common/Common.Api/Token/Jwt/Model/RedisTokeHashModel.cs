using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Api.Token.Jwt.Model;

public class RedisTokeHashModel
{
    /// <summary>
    /// 使用者此裝置目前有效的 Access Token
    /// </summary>
    public string AccessToken { get; set; } = default!;

    /// <summary>
    /// 此裝置對應的 Refresh Token
    /// </summary>
    public string RefreshToken { get; set; } = default!;

    /// <summary>
    /// Access Token 的過期時間 (UTC)
    /// </summary>
    public DateTime AccessTokenExpiresAt { get; set; }

    /// <summary>
    /// Refresh Token 的過期時間 (UTC)
    /// </summary>
    public DateTime RefreshTokenExpiresAt { get; set; }

    /// <summary>
    /// Token 的版本號，驗證時比對用（搭配 Redis 控制失效）
    /// </summary>
    public int TokenVersion { get; set; }

    /// <summary>
    /// 是否已被撤銷（例如後台登出）
    /// </summary>
    public bool IsRevoked { get; set; } = false;

    /// <summary>
    /// 此裝置登入時的來源 IP
    /// </summary>
    public string Ip { get; set; } = default!;

    /// <summary>
    /// 使用者代理字串（User-Agent）
    /// </summary>
    public string UserAgent { get; set; } = default!;

    /// <summary>
    /// 最近一次成功登入的時間 (UTC)
    /// </summary>
    public DateTime LastLoginAt { get; set; }
}
